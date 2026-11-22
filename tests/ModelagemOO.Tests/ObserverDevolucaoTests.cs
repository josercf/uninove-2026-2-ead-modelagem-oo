using Topico03;
using Topico04;
using Topico10.Antes;
using Topico10.Depois;

namespace ModelagemOO.Tests;

// Mesmo cenário nos dois desenhos: Joana devolve "Dom Casmurro" no prazo e com atraso.
public class ObserverDevolucaoTests
{
    private static readonly DateOnly Retirada = new(2026, 11, 2);
    private static readonly DateOnly Prevista = new(2026, 11, 17);

    private static (Usuario, Livro) Cena()
    {
        var joana = new Usuario("Joana", Perfil.Estudante);
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        livro.Emprestar(Retirada);
        return (joana, livro);
    }

    private static List<string> MensagensDoDesenhoDepois(DateOnly entrega, out Devolucao devolucao)
    {
        var (joana, livro) = Cena();
        var aviso = new AvisoAoUsuario();
        var painel = new PainelDoBalcao();
        devolucao = new Devolucao();
        devolucao.Inscrever(aviso);
        devolucao.Inscrever(painel);
        devolucao.Registrar(joana, livro, Prevista, entrega);
        return [.. aviso.Mensagens, .. painel.Mensagens];
    }

    [Fact]
    public void DevolucaoNoPrazoGeraAsMesmasMensagensNosDoisDesenhos()
    {
        var (joana, livro) = Cena();
        var antes = new DevolucaoComAvisoDireto();
        antes.Registrar(joana, livro, Prevista, Prevista);

        var depois = MensagensDoDesenhoDepois(Prevista, out _);

        Assert.Equal(
            ["Aviso a Joana: \"Dom Casmurro\" devolvido no prazo.", "Painel: \"Dom Casmurro\" disponível no acervo."],
            antes.Mensagens);
        Assert.Equal(antes.Mensagens, depois);
    }

    [Fact]
    public void DevolucaoComAtrasoGeraAsMesmasMensagensNosDoisDesenhos()
    {
        var (joana, livro) = Cena();
        var antes = new DevolucaoComAvisoDireto();
        var entrega = Prevista.AddDays(3);
        antes.Registrar(joana, livro, Prevista, entrega);

        var depois = MensagensDoDesenhoDepois(entrega, out _);

        Assert.Equal("Aviso a Joana: \"Dom Casmurro\" devolvido com 3 dia(s) de atraso.", antes.Mensagens[0]);
        Assert.Equal(antes.Mensagens, depois);
    }

    [Fact]
    public void ItemNaoEmprestadoELancadoNosDoisDesenhosESemMensagem()
    {
        var joana = new Usuario("Joana", Perfil.Estudante);
        var livro = new Livro("Dom Casmurro", "Machado de Assis");

        var antes = new DevolucaoComAvisoDireto();
        Assert.Throws<InvalidOperationException>(() => antes.Registrar(joana, livro, Prevista, Prevista));
        Assert.Empty(antes.Mensagens);

        var aviso = new AvisoAoUsuario();
        var depois = new Devolucao();
        depois.Inscrever(aviso);
        Assert.Throws<InvalidOperationException>(() => depois.Registrar(joana, livro, Prevista, Prevista));
        Assert.Empty(aviso.Mensagens);
    }

    // Terceiro interessado: entra por inscrição, sem editar a classe Devolucao.
    private sealed class ReservaDaFila : IObservadorDeDevolucao
    {
        public List<string> Chamados { get; } = [];
        public void Atualizar(DevolucaoRegistrada fato) => Chamados.Add(fato.Titulo);
    }

    [Fact]
    public void ObservadorNovoRecebeOAvisoSemAlterarADevolucao()
    {
        var (joana, livro) = Cena();
        var fila = new ReservaDaFila();
        var devolucao = new Devolucao();
        devolucao.Inscrever(fila);
        devolucao.Registrar(joana, livro, Prevista, Prevista);
        Assert.Equal(["Dom Casmurro"], fila.Chamados);
    }

    [Fact]
    public void ObservadorCancelamentoNaoRecebeMais()
    {
        var (joana, livro) = Cena();
        var fila = new ReservaDaFila();
        var devolucao = new Devolucao();
        devolucao.Inscrever(fila);
        devolucao.Cancelar(fila);
        devolucao.Registrar(joana, livro, Prevista, Prevista);
        Assert.Empty(fila.Chamados);
    }

    // Limite do padrão: um observador que falha interrompe os que vêm depois dele.
    private sealed class ObservadorQueFalha : IObservadorDeDevolucao
    {
        public void Atualizar(DevolucaoRegistrada fato) => throw new InvalidOperationException("falha do observador");
    }

    [Fact]
    public void ObservadorQueFalhaInterrompeOsSeguintes()
    {
        var (joana, livro) = Cena();
        var painel = new PainelDoBalcao();
        var devolucao = new Devolucao();
        devolucao.Inscrever(new ObservadorQueFalha());
        devolucao.Inscrever(painel);

        Assert.Throws<InvalidOperationException>(() => devolucao.Registrar(joana, livro, Prevista, Prevista));
        Assert.Empty(painel.Mensagens);
        // A devolução em si já tinha acontecido: o item voltou à estante.
        Assert.True(livro.Disponivel);
    }
}
