// Testes do tópico 01: a mesma regra escrita de forma procedural e orientada a objetos.
using Topico01;

public class EmprestimoTests
{
    private static readonly DateOnly Retirada = new(2026, 9, 21);

    [Fact]
    public void AsDuasVersoesCalculamAMesmaDataDeDevolucao()
    {
        var procedural = EmprestimoProcedural.CalcularDevolucao(Retirada, 15);
        var objeto = new Emprestimo("Dom Casmurro", Retirada, 15);
        Assert.Equal(new DateOnly(2026, 10, 6), procedural);
        Assert.Equal(procedural, objeto.Devolucao);
    }

    [Fact]
    public void VersaoProceduralAceitaPrazoZeroSemReclamar()
    {
        Assert.Equal(Retirada, EmprestimoProcedural.CalcularDevolucao(Retirada, 0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void ObjetoRecusaPrazoQueNaoEPositivo(int prazo)
    {
        var erro = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Emprestimo("Dom Casmurro", Retirada, prazo));
        Assert.Contains("maior que zero", erro.Message);
    }

    [Fact]
    public void ObjetoRecusaTituloVazio()
    {
        Assert.Throws<ArgumentException>(() => new Emprestimo("  ", Retirada, 15));
    }
}
