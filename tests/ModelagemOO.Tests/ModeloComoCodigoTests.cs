// Testes do tópico 12: o diagrama de classes regenerado do código C# contra o classes.puml versionado.
using Topico03;
using Topico04;
using Topico05;
using Topico06;
using Topico12.Desvio;
using Topico12.Geracao;
using Topico12.Medidas;

// Localiza a raiz do repositório, a pasta que contém ModelagemOO.sln, e lê arquivos a partir dela.
internal static class RepositorioParaTestes
{
    public static string Raiz()
    {
        var pasta = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(pasta.FullName, "ModelagemOO.sln")))
            pasta = pasta.Parent ?? throw new FileNotFoundException("ModelagemOO.sln não encontrado.");
        return pasta.FullName;
    }

    public static string Ler(params string[] caminho) =>
        File.ReadAllText(Path.Combine([Raiz(), .. caminho]));
}

public class ModeloComoCodigoTests
{
    // As oito classes que classes.puml (tópico 06) desenha.
    private static readonly Type[] TiposDoTopico06 =
    [
        typeof(ItemAcervo), typeof(Livro), typeof(Revista), typeof(Midia),
        typeof(Usuario), typeof(Perfil), typeof(Balcao), typeof(Emprestimo),
    ];

    private static string ClassesVersionado() =>
        RepositorioParaTestes.Ler("topico-06-classes-e-objetos", "classes.puml");

    [Fact]
    public void DiagramaGeradoDoCodigoNaoDivergeDoClassesPumlVersionado()
    {
        var gerado = GeradorDeDiagramaDeClasses.Gerar(TiposDoTopico06);
        var divergencias = ComparadorDeModelo.Comparar(ClassesVersionado(), gerado);
        // Assert.Empty mostra todas as divergências de uma vez, uma por linha.
        Assert.Empty(divergencias);
    }

    [Fact]
    public void ArquivoClassesGeradoVersionadoEhOQueOGeradorEscreveHoje()
    {
        // classes-gerado.puml é o arquivo que o programa grava; se o código mudar e ninguém regravar, o teste reprova.
        var versionado = RepositorioParaTestes.Ler("topico-12-pratica-de-modelagem", "classes-gerado.puml");
        Assert.Equal(GeradorDeDiagramaDeClasses.GerarArquivo(TiposDoTopico06).ReplaceLineEndings("\n"), versionado.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void GeradorEscreveClasseAbstrataComMembrosNaOrdemDoDiagrama()
    {
        var bloco = GeradorDeDiagramaDeClasses.GerarBloco(typeof(ItemAcervo), TiposDoTopico06.ToHashSet());
        var esperado = string.Join('\n',
            "abstract class ItemAcervo {",
            "  -_emprestado : bool",
            "  +Titulo : string",
            "  +Disponivel : bool",
            "  +{abstract} PrazoEmDias : int",
            "  #ItemAcervo(titulo : string)",
            "  +Emprestar(retirada : DateOnly) : DateOnly",
            "  +Devolver() : void",
            "}");
        Assert.Equal(esperado, bloco);
    }

    [Fact]
    public void GeradorEscreveEnumeracaoComSeusValores()
    {
        var bloco = GeradorDeDiagramaDeClasses.GerarBloco(typeof(Perfil), TiposDoTopico06.ToHashSet());
        Assert.Equal("enum Perfil <<enumeration>> {\n  Morador\n  Estudante\n  Voluntario\n}", bloco);
    }

    [Fact]
    public void PropriedadeDeUmaClasseDoModeloViraAssociacaoENaoMembro()
    {
        // Emprestimo.Usuario e Emprestimo.Item apontam para classes do modelo: ficam fora da caixa.
        var bloco = GeradorDeDiagramaDeClasses.GerarBloco(typeof(Emprestimo), TiposDoTopico06.ToHashSet());
        Assert.DoesNotContain("Usuario", bloco);
        Assert.Contains("+Retirada : DateOnly", bloco);
        // Usuario.Perfil é enumeração, e o valor de enumeração continua como membro.
        var usuario = GeradorDeDiagramaDeClasses.GerarBloco(typeof(Usuario), TiposDoTopico06.ToHashSet());
        Assert.Contains("+Perfil : Perfil", usuario);
    }

    [Fact]
    public void GeradorEscreveUmaLinhaDeHerancaPorSubclasse()
    {
        var gerado = GeradorDeDiagramaDeClasses.Gerar(TiposDoTopico06);
        Assert.Contains("ItemAcervo <|-- Livro", gerado);
        Assert.Contains("ItemAcervo <|-- Revista", gerado);
        Assert.Contains("ItemAcervo <|-- Midia", gerado);
    }

    [Fact]
    public void CodigoQueDivergeDoModeloEhApontadoMembroAMembro()
    {
        // UsuarioQueDivergiu faz o papel de um Usuario editado sem atualizar o diagrama:
        // ganhou Suspender(), perdeu QuitarMulta() e mudou o tipo de Limite para long.
        var bloco = GeradorDeDiagramaDeClasses.GerarBloco(
            typeof(UsuarioQueDivergiu), TiposDoTopico06.ToHashSet(), nomeNoDiagrama: "Usuario");
        var divergencias = ComparadorDeModelo.Comparar(ClassesVersionado(), bloco);

        var textos = divergencias.Where(d => d.Classe == "Usuario").Select(d => d.ToString()).ToList();
        Assert.Contains("Usuario: membro ausente no diagrama: +Suspender() : void", textos);
        Assert.Contains("Usuario: membro ausente no código: +QuitarMulta() : void", textos);
        Assert.Contains("Usuario: membro ausente no código: +Limite : int", textos);
        Assert.Contains("Usuario: membro ausente no diagrama: +Limite : long", textos);
        Assert.Equal(4, textos.Count);
    }

    [Fact]
    public void ClasseDesenhadaQueNaoExisteNoCodigoEhApontada()
    {
        // Só ItemAcervo no código: as outras sete classes do diagrama ficam sem correspondente.
        var gerado = GeradorDeDiagramaDeClasses.Gerar([typeof(ItemAcervo)]);
        var divergencias = ComparadorDeModelo.Comparar(ClassesVersionado(), gerado);
        Assert.Equal(7, divergencias.Count(d => d.Natureza == "classe ausente no código"));
    }

    [Fact]
    public void ClasseNovaNoCodigoSemDesenhoEhApontada()
    {
        var gerado = GeradorDeDiagramaDeClasses.Gerar([.. TiposDoTopico06, typeof(UsuarioQueDivergiu)]);
        var divergencias = ComparadorDeModelo.Comparar(ClassesVersionado(), gerado);
        Assert.Contains(divergencias, d => d.Classe == "UsuarioQueDivergiu" && d.Natureza == "classe ausente no diagrama");
    }

    [Fact]
    public void EngenhariaReversaRecuperaASomaDosBlocosEDasHerancasENadaAlemDisso()
    {
        var cobertura = CoberturaDaEngenhariaReversa.Medir(ClassesVersionado(), GeradorDeDiagramaDeClasses.Gerar(TiposDoTopico06));
        // Medido em classes.puml: 55 linhas de modelo, das quais o gerador reproduz 47.
        Assert.Equal(LINHAS_GERADAS, cobertura.LinhasGeradas);
        Assert.Equal(LINHAS_DE_MODELO, cobertura.LinhasDeModelo);
        // As 8 restantes são 2 associações, 1 linha oculta, 1 dependência e as 4 linhas da nota: o código não as diz.
        Assert.Equal(LINHAS_DE_MODELO - LINHAS_GERADAS, cobertura.LinhasNaoRecuperaveis.Count);
    }

    private const int LINHAS_GERADAS = 47;
    private const int LINHAS_DE_MODELO = 55;

    [Fact]
    public void InventarioContaOitoArquivosDeModeloDosTopicos05A08()
    {
        var inventario = InventarioDeDiagramas.Ler(RepositorioParaTestes.Raiz());
        Assert.Equal(8, inventario.Arquivos.Count);
        Assert.All(inventario.Arquivos, a => Assert.True(a.Linhas > 0 && a.Bytes > 0));
        Assert.Equal(372, inventario.Arquivos.Sum(a => a.Linhas));
        // Os arquivos são texto: o diff de uma mudança de linha é linha a linha.
        Assert.Contains(inventario.Arquivos, a => a.Nome == "classes.puml");
    }

    [Fact]
    public void RenomearUsuarioEmTextoExigeEditarCadaOcorrenciaNosArquivos()
    {
        var inventario = InventarioDeDiagramas.Ler(RepositorioParaTestes.Raiz());
        var (linhas, arquivos) = inventario.OcorrenciasDe("Usuario");
        // Medido nos 8 arquivos por dotnet run: 19 linhas em 4 arquivos.
        Assert.Equal(OCORRENCIAS_LINHAS, linhas);
        Assert.Equal(OCORRENCIAS_ARQUIVOS, arquivos);
    }

    private const int OCORRENCIAS_LINHAS = 19;
    private const int OCORRENCIAS_ARQUIVOS = 4;
}
