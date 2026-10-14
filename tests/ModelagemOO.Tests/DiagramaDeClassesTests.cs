// Testes do tópico 06: a classe Emprestimo e a conferência do diagrama contra o código.
using System.Reflection;
using System.Text.RegularExpressions;
using Topico03;
using Topico04;
using Topico05;
using Topico06;

public class DiagramaDeClassesTests
{
    private static readonly DateOnly Retirada = new(2026, 10, 5);

    [Fact]
    public void EmprestimoExigeExatamenteUmUsuario()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        // Multiplicidade 1 do lado do Usuario: sem usuário, o objeto não é criado.
        Assert.Throws<ArgumentNullException>(() => new Emprestimo(null!, livro, Retirada));
    }

    [Fact]
    public void EmprestimoExigeExatamenteUmItem()
    {
        var joana = new Usuario("Joana", Perfil.Morador);
        // Multiplicidade 1 do lado do ItemAcervo: sem item, o objeto não é criado.
        Assert.Throws<ArgumentNullException>(() => new Emprestimo(joana, null!, Retirada));
    }

    [Fact]
    public void DevolucaoVemDoPrazoDaSubclasse()
    {
        var joana = new Usuario("Joana", Perfil.Morador);
        var livro = new Emprestimo(joana, new Livro("Dom Casmurro", "Machado de Assis"), Retirada);
        var revista = new Emprestimo(joana, new Revista("Superinteressante", 480), Retirada);
        // Livro tem prazo de 15 dias e Revista de 7, pelas regras do README da raiz.
        Assert.Equal(new DateOnly(2026, 10, 20), livro.Devolucao);
        Assert.Equal(new DateOnly(2026, 10, 12), revista.Devolucao);
    }

    [Fact]
    public void UsuarioMoradorNaoPassaDeDoisEmprestimos()
    {
        var joana = new Usuario("Joana", Perfil.Morador);
        new Emprestimo(joana, new Livro("Dom Casmurro", "Machado de Assis"), Retirada);
        new Emprestimo(joana, new Revista("Superinteressante", 480), Retirada);
        var midia = new Midia("Central do Brasil", "DVD");
        // O limite do perfil (RF01) é o teto da multiplicidade 0..6 do diagrama.
        Assert.Throws<InvalidOperationException>(() => new Emprestimo(joana, midia, Retirada));
        // A recusa não deixa o item marcado como emprestado.
        Assert.True(midia.Disponivel);
    }

    [Fact]
    public void ItemEmprestadoNaoEntraEmOutroEmprestimo()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        new Emprestimo(new Usuario("Joana", Perfil.Morador), livro, Retirada);
        var caio = new Usuario("Caio", Perfil.Estudante);
        // Enquanto o item está emprestado, Disponivel é false e o novo empréstimo é recusado.
        Assert.Throws<InvalidOperationException>(() => new Emprestimo(caio, livro, Retirada));
        // A recusa não acrescenta o título à lista de itens de Caio.
        Assert.Empty(caio.Itens);
    }

    [Fact]
    public void ItemDevolvidoEntraEmNovoEmprestimo()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        var primeiro = new Emprestimo(new Usuario("Joana", Perfil.Morador), livro, Retirada);
        livro.Devolver();
        var segundo = new Emprestimo(new Usuario("Caio", Perfil.Estudante), livro, Retirada.AddDays(20));
        // Multiplicidade 0..* do lado do Emprestimo: o mesmo item acumula empréstimos ao longo do tempo.
        Assert.Same(primeiro.Item, segundo.Item);
    }

    [Fact]
    public void CadaMembroDoDiagramaExisteNoCodigoComAMesmaVisibilidade()
    {
        var problemas = new List<string>();
        // Cada membro desenhado em classes.puml vira uma tupla (classe, visibilidade, nome, tipo).
        foreach (var (classe, visibilidade, nome, tipo) in MembrosDoDiagrama())
        {
            // A reflexão procura, na classe C# de mesmo nome, o membro com o nome desenhado.
            var membro = BuscarMembro(TipoPorNome(classe), nome);
            // Primeiro desvio possível: o membro desenhado não existe no código.
            if (membro is null)
                problemas.Add($"{classe}.{nome}: ausente no código");
            // Segundo: o símbolo + - ou # não corresponde a public, private ou protected.
            else if (Visibilidade(membro) != visibilidade)
                problemas.Add($"{classe}.{nome}: diagrama '{visibilidade}', código '{Visibilidade(membro)}'");
            // Terceiro: o tipo depois dos dois-pontos difere do tipo C#; construtor não tem tipo.
            else if (tipo != "" && NomeCSharp(TipoDe(membro)) != tipo)
                problemas.Add($"{classe}.{nome}: diagrama '{tipo}', código '{NomeCSharp(TipoDe(membro))}'");
        }
        // Assert.Empty lista todos os desvios de uma vez quando a lista não está vazia.
        Assert.Empty(problemas);
    }

    [Fact]
    public void CadaHerancaDoDiagramaExisteNoCodigo()
    {
        var herancas = Regex.Matches(LerDiagrama(), @"^(\w+) <\|-- (\w+)", RegexOptions.Multiline);
        // O diagrama desenha três heranças: Livro, Revista e Midia a partir de ItemAcervo.
        Assert.Equal(3, herancas.Count);
        foreach (Match h in herancas)
            Assert.Equal(TipoPorNome(h.Groups[1].Value), TipoPorNome(h.Groups[2].Value).BaseType);
    }

    [Fact]
    public void CadaValorDaEnumeracaoDoDiagramaExisteNoCodigo()
    {
        var corpo = Regex.Match(LerDiagrama(), @"enum Perfil <<enumeration>> \{(.*?)\}", RegexOptions.Singleline);
        var valores = corpo.Groups[1].Value.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Assert.Equal(Enum.GetNames<Perfil>(), valores);
    }

    [Fact]
    public void DiagramaDeObjetosUsaSoAtributosEPapeisDoDiagramaDeClasses()
    {
        var problemas = new List<string>();
        var objetos = LerDiagrama("objetos.puml");
        // Papéis das associações, como "Emprestimo.Usuario"; o resto dos membros fica nas caixas.
        var papeis = Regex.Matches(LerDiagrama(), @"^(\w+)(?: ""[^""]*"")? --> ""[+\-#](\w+) ", RegexOptions.Multiline)
            .Select(a => $"{a.Groups[1].Value}.{a.Groups[2].Value}").ToHashSet();
        var membros = MembrosDoDiagrama().Select(m => $"{m.Classe}.{m.Nome}").ToHashSet();
        // Classe base de cada subclasse, lida das linhas "ItemAcervo <|-- Livro".
        var bases = Regex.Matches(LerDiagrama(), @"^(\w+) <\|-- (\w+)", RegexOptions.Multiline)
            .ToDictionary(h => h.Groups[2].Value, h => h.Groups[1].Value);
        // Procura "Classe.nome" no conjunto e, pela herança, na classe base.
        bool Tem(HashSet<string> conjunto, string classe, string nome) =>
            conjunto.Contains($"{classe}.{nome}") || (bases.TryGetValue(classe, out var b) && Tem(conjunto, b, nome));
        // Valor de objeto exige atributo na caixa; vínculo exige papel de associação.
        bool Atributo(string classe, string nome) => Tem(membros, classe, nome) && !Tem(papeis, classe, nome);
        bool Papel(string classe, string nome) => Tem(papeis, classe, nome);
        // Exemplo de bloco: object "<u>joana : Usuario</u>" as joana { Nome = "Joana" ... }.
        var classeDe = new Dictionary<string, string>();
        foreach (Match o in Regex.Matches(objetos, @"^object ""<u>\w+ : (\w+)</u>"" as (\w+) \{(.*?)^\}", RegexOptions.Multiline | RegexOptions.Singleline))
        {
            classeDe[o.Groups[2].Value] = o.Groups[1].Value;
            // Cada linha "Atributo = valor" precisa ser um atributo desenhado na classe do objeto.
            foreach (Match v in Regex.Matches(o.Groups[3].Value, @"^\s*(\w+) = ", RegexOptions.Multiline))
                if (!Atributo(o.Groups[1].Value, v.Groups[1].Value))
                    problemas.Add($"{o.Groups[2].Value}.{v.Groups[1].Value}: não é atributo de {o.Groups[1].Value}");
        }
        // Exemplo de vínculo: e1 --> "Usuario" joana. O rótulo precisa ser papel de uma associação.
        foreach (Match l in Regex.Matches(objetos, @"^(\w+) --> ""(\w+)"" (\w+)\s*$", RegexOptions.Multiline))
            if (!Papel(classeDe[l.Groups[1].Value], l.Groups[2].Value))
                problemas.Add($"vínculo {l.Groups[1].Value}.{l.Groups[2].Value}: papel ausente em {classeDe[l.Groups[1].Value]}");
        // Guarda contra um arquivo lido pela metade: o retrato de Joana tem cinco objetos.
        Assert.Equal(5, classeDe.Count);
        Assert.Empty(problemas);
    }

    // Procura o arquivo (classes.puml, se nenhum for informado) subindo as pastas a partir
    // da pasta onde o teste roda, até achar a raiz do repositório (a que contém ModelagemOO.sln).
    private static string LerDiagrama(string arquivo = "classes.puml")
    {
        var pasta = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(pasta.FullName, "ModelagemOO.sln")))
            pasta = pasta.Parent ?? throw new FileNotFoundException("ModelagemOO.sln não encontrado.");
        return File.ReadAllText(Path.Combine(pasta.FullName, "topico-06-classes-e-objetos", arquivo));
    }

    // Lê cada bloco "class X {" ou "abstract class X {" e devolve os membros com + - ou #.
    // Lê também a ponta de cada associação, em que o papel vira propriedade da classe de origem.
    private static List<(string Classe, char Visibilidade, string Nome, string Tipo)> MembrosDoDiagrama()
    {
        var diagrama = LerDiagrama();
        var membros = new List<(string, char, string, string)>();
        var blocos = Regex.Matches(diagrama, @"^(?:abstract )?class (\w+) \{(.*?)^\}", RegexOptions.Multiline | RegexOptions.Singleline);
        foreach (Match bloco in blocos)
        {
            // Exemplos de linha: "+Titulo : string", "#ItemAcervo(titulo : string)".
            var linhas = Regex.Matches(bloco.Groups[2].Value, @"^\s*([+\-#])(?:\{abstract\} )?(\w+)(\([^)]*\))?(?: : (.+?))?\s*$", RegexOptions.Multiline);
            foreach (Match l in linhas)
                membros.Add((bloco.Groups[1].Value, l.Groups[1].Value[0], l.Groups[2].Value, l.Groups[4].Value));
        }
        // Exemplo de linha: Emprestimo "0..6" --> "+Usuario 1" Usuario.
        var associacoes = Regex.Matches(diagrama, @"^(\w+)(?: ""[^""]*"")? --> ""([+\-#])(\w+) [^""]*"" (\w+)\s*$", RegexOptions.Multiline);
        foreach (Match a in associacoes)
            membros.Add((a.Groups[1].Value, a.Groups[2].Value[0], a.Groups[3].Value, a.Groups[4].Value));
        // Guarda contra um diagrama lido pela metade: hoje são 25 membros nas caixas e 2 papéis nas associações.
        Assert.True(membros.Count >= 27, $"só {membros.Count} membros lidos de classes.puml");
        return membros;
    }

    private static Type TipoPorNome(string nome) =>
        new[] { typeof(ItemAcervo).Assembly, typeof(Usuario).Assembly, typeof(Balcao).Assembly, typeof(Emprestimo).Assembly }
            .SelectMany(a => a.GetTypes())
            .Single(t => t.Name == nome);

    private const BindingFlags Todos = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static MemberInfo? BuscarMembro(Type tipo, string nome) =>
        nome == tipo.Name
            ? tipo.GetConstructors(Todos).FirstOrDefault()
            : (MemberInfo?)tipo.GetProperty(nome, Todos) ?? (MemberInfo?)tipo.GetMethod(nome, Todos) ?? tipo.GetField(nome, Todos);

    // Traduz o modificador de acesso do C# para o símbolo de visibilidade da UML.
    private static char Visibilidade(MemberInfo membro)
    {
        MethodBase? metodo = membro switch
        {
            PropertyInfo p => p.GetMethod,
            MethodBase m => m,
            _ => null,
        };
        if (metodo is not null)
            return metodo.IsPublic ? '+' : metodo.IsFamily ? '#' : '-';
        var campo = (FieldInfo)membro;
        return campo.IsPublic ? '+' : campo.IsFamily ? '#' : '-';
    }

    private static Type TipoDe(MemberInfo membro) => membro switch
    {
        PropertyInfo p => p.PropertyType,
        MethodInfo m => m.ReturnType,
        FieldInfo f => f.FieldType,
        _ => typeof(void),
    };

    private static readonly Dictionary<string, string> Apelidos = new()
    {
        ["String"] = "string", ["Boolean"] = "bool", ["Int32"] = "int", ["Void"] = "void",
    };

    // Escreve o tipo como no C#: List`1 de String vira List<string>.
    private static string NomeCSharp(Type t) => t.IsGenericType
        ? $"{t.Name[..t.Name.IndexOf('`')]}<{string.Join(", ", t.GetGenericArguments().Select(NomeCSharp))}>"
        : Apelidos.GetValueOrDefault(t.Name, t.Name);
}
