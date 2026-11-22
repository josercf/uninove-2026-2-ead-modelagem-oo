using Topico03;
using Topico10.Antes;
using Topico10.Depois;

namespace ModelagemOO.Tests;

// Mesmo cenário nos dois desenhos: o cadastro cria o tipo certo de item do acervo.
public class FactoryItensTests
{
    public static TheoryData<string, string, string, Type, int> Cenario => new()
    {
        { "livro", "Dom Casmurro", "Machado de Assis", typeof(Livro), 15 },
        { "revista", "Piauí", "120", typeof(Revista), 7 },
        { "midia", "Cidade de Deus", "DVD", typeof(Midia), 3 },
        { "LIVRO", "Quincas Borba", "Machado de Assis", typeof(Livro), 15 },
    };

    [Theory]
    [MemberData(nameof(Cenario))]
    public void DesenhoAntesCriaOTipoCerto(string tipo, string titulo, string complemento, Type esperado, int prazo)
    {
        var item = CadastroComSwitch.Cadastrar(tipo, titulo, complemento);
        Assert.IsType(esperado, item);
        Assert.Equal(titulo, item.Titulo);
        Assert.Equal(prazo, item.PrazoEmDias);
    }

    [Theory]
    [MemberData(nameof(Cenario))]
    public void DesenhoDepoisCriaOTipoCerto(string tipo, string titulo, string complemento, Type esperado, int prazo)
    {
        var item = CadastroDoAcervo.ComFabricasPadrao().Cadastrar(tipo, titulo, complemento);
        Assert.IsType(esperado, item);
        Assert.Equal(titulo, item.Titulo);
        Assert.Equal(prazo, item.PrazoEmDias);
    }

    [Fact]
    public void TipoDesconhecidoTemAMesmaMensagemNosDoisDesenhos()
    {
        var antes = Assert.Throws<ArgumentException>(() => CadastroComSwitch.Cadastrar("fita", "Titanic", "VHS"));
        var depois = Assert.Throws<ArgumentException>(
            () => CadastroDoAcervo.ComFabricasPadrao().Cadastrar("fita", "Titanic", "VHS"));
        Assert.Equal("Tipo de item desconhecido: fita.", antes.Message);
        Assert.Equal(antes.Message, depois.Message);
    }

    [Fact]
    public void EdicaoQueNaoENumeroTemAMesmaMensagemNosDoisDesenhos()
    {
        var antes = Assert.Throws<ArgumentException>(() => CadastroComSwitch.Cadastrar("revista", "Piauí", "cento e vinte"));
        var depois = Assert.Throws<ArgumentException>(
            () => CadastroDoAcervo.ComFabricasPadrao().Cadastrar("revista", "Piauí", "cento e vinte"));
        Assert.Equal("A edição da revista deve ser um número inteiro.", antes.Message);
        Assert.Equal(antes.Message, depois.Message);
    }

    [Fact]
    public void TituloVazioERecusadoPelaClasseBaseNosDoisDesenhos()
    {
        Assert.Throws<ArgumentException>(() => CadastroComSwitch.Cadastrar("livro", " ", "Autor"));
        Assert.Throws<ArgumentException>(() => CadastroDoAcervo.ComFabricasPadrao().Cadastrar("livro", " ", "Autor"));
    }

    // Ponto de extensão: tipo novo entra por fábrica registrada, sem alterar o CadastroDoAcervo.
    private sealed class Jogo(string titulo) : ItemAcervo(titulo)
    {
        public override int PrazoEmDias => 5;
    }

    private sealed class FabricaDeJogo : FabricaDeItem
    {
        public override ItemAcervo Criar(string titulo, string complemento) => new Jogo(titulo);
    }

    [Fact]
    public void TipoNovoEntraPorFabricaRegistrada()
    {
        var cadastro = CadastroDoAcervo.ComFabricasPadrao();
        cadastro.Registrar("jogo", new FabricaDeJogo());
        var item = cadastro.Cadastrar("jogo", "Dominó", "");
        Assert.Equal(5, item.PrazoEmDias);
    }

    [Fact]
    public void RegistrarOMesmoTipoDuasVezesEhRecusado()
    {
        var cadastro = CadastroDoAcervo.ComFabricasPadrao();
        Assert.Throws<InvalidOperationException>(() => cadastro.Registrar("livro", new FabricaDeLivro()));
    }
}
