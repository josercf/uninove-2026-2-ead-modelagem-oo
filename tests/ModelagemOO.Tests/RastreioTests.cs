// Testes do tópico 09: o rastreio do caso de uso ao modelo de domínio e ao código, conferido contra os arquivos.
using System.Reflection;
using System.Text.RegularExpressions;

public class RastreioTests
{
    private static string Raiz()
    {
        var pasta = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(pasta.FullName, "ModelagemOO.sln")))
            pasta = pasta.Parent ?? throw new FileNotFoundException("ModelagemOO.sln não encontrado.");
        return pasta.FullName;
    }

    private static string Ler(params string[] caminho) => File.ReadAllText(Path.Combine([Raiz(), .. caminho]));

    private static readonly string Rastreio = Ler("topico-09-analise-e-projeto", "RASTREIO.md");
    private static readonly string Dominio = Ler("topico-09-analise-e-projeto", "modelo-de-dominio.puml");
    private static readonly string CasoDeUso = Ler("topico-05-casos-de-uso", "CASOS_DE_USO.md");

    // Assemblies dos tópicos, alcançados por um tipo de cada um.
    private static readonly Assembly[] Montagens =
    [
        typeof(Topico03.ItemAcervo).Assembly, typeof(Topico04.Usuario).Assembly, typeof(Topico05.Balcao).Assembly,
        typeof(Topico06.Emprestimo).Assembly, typeof(Topico09.Depois.Balcao).Assembly,
    ];

    private static Type? Tipo(string nomeCompleto) =>
        Montagens.Select(m => m.GetType(nomeCompleto)).FirstOrDefault(t => t is not null);

    private static bool TesteExiste(string metodo) =>
        typeof(RastreioTests).Assembly.GetTypes().Any(t => t.GetMethod(metodo) is not null);

    // Células das linhas de dados da tabela que vem logo depois do título "## <secao>".
    private static List<string[]> Linhas(string secao)
    {
        var bloco = Regex.Match(Rastreio, $@"^## {Regex.Escape(secao)}\s*$(.*?)(?=^## |\z)", RegexOptions.Multiline | RegexOptions.Singleline);
        Assert.True(bloco.Success, $"seção '{secao}' não encontrada em RASTREIO.md");
        return bloco.Groups[1].Value.Split('\n')
            .Select(l => l.Trim())
            .Where(l => l.StartsWith('|') && !Regex.IsMatch(l, @"^\|[\s\-|]+\|$"))
            .Skip(1) // cabeçalho
            .Select(l => l.Trim('|').Split('|').Select(c => c.Trim().Trim('`')).ToArray())
            .ToList();
    }

    [Fact]
    public void ATabelaDeConceitosAdotadosTemQuatroLinhasDeCincoColunas()
    {
        var linhas = Linhas("Conceitos adotados");
        Assert.Equal(4, linhas.Count);
        Assert.All(linhas, l => Assert.Equal(5, l.Length));
    }

    [Fact]
    public void CadaTermoDoRastreioApareceNoTextoDoCasoDeUso()
    {
        var termos = Linhas("Conceitos adotados").Select(l => l[0])
            .Concat(Linhas("Candidatas descartadas").Select(l => l[0]));
        foreach (var termo in termos)
            Assert.Contains(termo, CasoDeUso, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CadaClasseDeProjetoDoRastreioExisteNoCodigo()
    {
        var nomes = Linhas("Conceitos adotados").Select(l => l[3])
            .Concat(Linhas("Classes de projeto sem classe de domínio").Select(l => l[1]));
        foreach (var nome in nomes)
            Assert.True(Tipo(nome) is not null, $"{nome} não existe nos assemblies dos tópicos");
    }

    [Fact]
    public void CadaTesteCitadoNoRastreioExisteNaSuite()
    {
        var testes = Linhas("Conceitos adotados").Select(l => l[4])
            .Concat(Linhas("Classes de projeto sem classe de domínio").Select(l => l[3]));
        foreach (var teste in testes)
            Assert.True(TesteExiste(teste), $"teste {teste} não existe");
    }

    [Fact]
    public void ASequenciaDoFluxoPrincipalFicaCobertaPeloRastreio()
    {
        var passos = Linhas("Conceitos adotados").Select(l => l[1])
            .Concat(Linhas("Classes de projeto sem classe de domínio").Select(l => l[0]))
            .SelectMany(p => Regex.Matches(p, @"\d").Select(m => int.Parse(m.Value)))
            .ToHashSet();
        foreach (var passo in new[] { 1, 2, 3, 4, 5 })
            Assert.Contains(passo, passos);
    }

    [Fact]
    public void ModeloDeDominioDeclaraExatamenteAsClassesAdotadas()
    {
        var noDiagrama = Regex.Matches(Dominio, @"^class (\w+)\s*$", RegexOptions.Multiline).Select(m => m.Groups[1].Value).Order();
        var adotadas = Linhas("Conceitos adotados").Select(l => l[2]).Order();
        Assert.Equal(adotadas, noDiagrama);
    }

    [Fact]
    public void ModeloDeDominioNaoTemMetodoNemVisibilidade()
    {
        // Modelo de domínio é conceitual: nome de classe e associação, sem operação.
        Assert.DoesNotMatch(@"\(\s*\)", Dominio);
        Assert.DoesNotMatch(@"(?m)^\s*[+\-#]\s*\w", Dominio);
    }

    [Fact]
    public void CadaCandidataDescartadaTemMotivoNoRastreioENoDiagrama()
    {
        var descartadasNoDiagrama = Regex.Matches(Dominio, @"^' descartado: (\w+) : (.+)$", RegexOptions.Multiline)
            .Select(m => m.Groups[1].Value).Order().ToList();
        var linhas = Linhas("Candidatas descartadas");
        Assert.Equal(5, linhas.Count);
        Assert.Equal(linhas.Select(l => l[1]).Order(), descartadasNoDiagrama);
        Assert.All(linhas, l => Assert.True(l[3].Length >= 20, $"motivo curto demais em {l[1]}"));
    }

    [Fact]
    public void ModeloAdotaQuatroDeNoveCandidatas()
    {
        var adotadas = Linhas("Conceitos adotados").Count;
        var descartadas = Linhas("Candidatas descartadas").Count;
        Assert.Equal(4, adotadas);
        Assert.Equal(9, adotadas + descartadas);
    }

    [Theory]
    [InlineData("Atendente")]
    [InlineData("Multa")]
    [InlineData("Pendencia")]
    [InlineData("Limite")]
    [InlineData("Reserva")]
    public void CandidataDescartadaNaoViraClasseNoCodigo(string candidata)
    {
        // Antecipar uma classe sem comportamento é o custo que o rastreio existe para barrar.
        foreach (var montagem in Montagens)
            Assert.DoesNotContain(montagem.GetTypes(), t => t.Name == candidata);
    }

    [Fact]
    public void NenhumaClasseDeDominioAdotadaFicaSemTesteNemSemPasso()
    {
        foreach (var l in Linhas("Conceitos adotados"))
        {
            Assert.False(string.IsNullOrWhiteSpace(l[1]), $"{l[0]} sem passo do caso de uso");
            Assert.False(string.IsNullOrWhiteSpace(l[4]), $"{l[0]} sem teste");
        }
    }
}
