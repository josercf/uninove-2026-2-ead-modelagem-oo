// Testes do tópico 08: componentes.puml contra os ProjectReference, implantacao.puml contra os projetos.
using System.Text.RegularExpressions;

public class ComponentesEImplantacaoTests
{
    private static string Raiz()
    {
        var pasta = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(pasta.FullName, "ModelagemOO.sln")))
            pasta = pasta.Parent ?? throw new FileNotFoundException("ModelagemOO.sln não encontrado.");
        return pasta.FullName;
    }

    private static string LerDiagrama(string arquivo) =>
        File.ReadAllText(Path.Combine(Raiz(), "topico-08-estados-componentes-implantacao", arquivo));

    // Procura o .csproj do componente em qualquer pasta do repositório, fora de bin e obj.
    private static string CsprojDe(string componente) =>
        Directory.GetFiles(Raiz(), $"{componente}.csproj", SearchOption.AllDirectories)
            .Single(c => !c.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                      && !c.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"));

    // Componentes declarados como: component [Nome] as ALIAS
    private static Dictionary<string, string> ComponentesDoDiagrama() =>
        Regex.Matches(LerDiagrama("componentes.puml"), @"^component \[(.+)\] as (\w+)\s*$", RegexOptions.Multiline)
            .ToDictionary(m => m.Groups[2].Value, m => m.Groups[1].Value);

    private static List<(string De, string Para)> SetasDoDiagrama()
    {
        var alias = ComponentesDoDiagrama();
        return Regex.Matches(LerDiagrama("componentes.puml"), @"^(\w+) --> (\w+)\s*$", RegexOptions.Multiline)
            .Select(m => (alias[m.Groups[1].Value], alias[m.Groups[2].Value]))
            .ToList();
    }

    private static List<string> ReferenciasDe(string componente) =>
        Regex.Matches(File.ReadAllText(CsprojDe(componente)), @"ProjectReference Include=""[^""]*?([\w.]+)\.csproj""")
            .Select(m => m.Groups[1].Value)
            .ToList();

    [Fact]
    public void OsComponentesDoDiagramaSaoProjetosDoRepositorio()
    {
        var componentes = ComponentesDoDiagrama().Values.ToList();
        Assert.Equal(6, componentes.Count);
        foreach (var c in componentes)
            Assert.True(File.Exists(CsprojDe(c)), $"{c}.csproj não existe no repositório");
    }

    [Fact]
    public void CadaSetaDeComponentesEhUmProjectReferenceEViceVersa()
    {
        var componentes = ComponentesDoDiagrama().Values.ToHashSet();
        var doDiagrama = SetasDoDiagrama().OrderBy(s => s).ToList();
        // Dos .csproj, só as dependências entre componentes desenhados (o teste referencia outros tópicos não desenhados).
        var doCodigo = componentes
            .SelectMany(c => ReferenciasDe(c).Where(componentes.Contains).Select(r => (c, r)))
            .OrderBy(s => s).ToList();
        Assert.True(doDiagrama.Count >= 14, $"só {doDiagrama.Count} setas lidas de componentes.puml");
        Assert.Equal(doCodigo, doDiagrama);
    }

    [Fact]
    public void ODiagramaNaoTemCicloDeDependencia()
    {
        var setas = SetasDoDiagrama();
        // Quem depende de outro componente nunca é dependência dele: sem seta nos dois sentidos.
        foreach (var (de, para) in setas)
            Assert.DoesNotContain((para, de), setas);
    }

    [Fact]
    public void OTopico08NaoReferenciaOsTestes()
    {
        Assert.DoesNotContain("ModelagemOO.Tests", ReferenciasDe("Topico08"));
    }

    [Fact]
    public void OsArtefatosDaImplantacaoExistemNoRepositorio()
    {
        var texto = LerDiagrama("implantacao.puml");
        var artefatos = Regex.Matches(texto, @"^\s*artifact ""([^""]+)""", RegexOptions.Multiline)
            .Select(m => m.Groups[1].Value).ToList();
        Assert.True(artefatos.Count >= 6, $"só {artefatos.Count} artefatos lidos de implantacao.puml");
        foreach (var a in artefatos.Where(a => a.EndsWith(".puml")))
            Assert.True(File.Exists(Path.Combine(Raiz(), "topico-08-estados-componentes-implantacao", a)), $"{a} não existe");
        // Cada .dll corresponde a um projeto do repositório, e o CI repete o artefato de testes.
        foreach (var dll in artefatos.Where(a => a.EndsWith(".dll")).Select(a => a.Split(' ')[0]).Distinct())
            Assert.True(File.Exists(CsprojDe(dll[..^4])), $"{dll} não tem projeto");
    }

    [Fact]
    public void ImplantacaoNaoDeclaraServidorNemBancoDeDados()
    {
        // O repositório não tem servidor de aplicação nem banco: o diagrama não pode desenhar nenhum.
        var texto = LerDiagrama("implantacao.puml");
        var codigo = string.Join("\n", texto.Split('\n').Where(l => !l.TrimStart().StartsWith('\'')));
        Assert.DoesNotMatch(@"(?i)database|servidor de aplica|sql|kestrel|nginx", codigo);
    }

    [Fact]
    public void OExecutorDaIntegracaoContinuaDesenhadoExisteNoWorkflow()
    {
        var workflow = File.ReadAllText(Path.Combine(Raiz(), ".github", "workflows", "ci.yml"));
        Assert.Contains("runs-on: ubuntu-latest", workflow);
        Assert.Contains("ubuntu-latest", LerDiagrama("implantacao.puml"));
        Assert.Contains("dotnet test", workflow);
    }
}
