// Testes do tópico 03: herança de ItemAcervo e prazo polimórfico por tipo.
using Topico03;

public class ItemAcervoTests
{
    private static readonly DateOnly Retirada = new(2026, 10, 5);

    [Fact]
    public void CadaTipoTemOPrazoDaRegraDoDominio()
    {
        Assert.Equal(15, new Livro("Dom Casmurro", "Machado de Assis").PrazoEmDias);
        Assert.Equal(7, new Revista("Superinteressante", 480).PrazoEmDias);
        Assert.Equal(3, new Midia("Central do Brasil", "DVD").PrazoEmDias);
    }

    [Fact]
    public void EmprestarDevolveDataCalculadaPeloTipoReal()
    {
        List<ItemAcervo> itens =
        [
            new Livro("Dom Casmurro", "Machado de Assis"),
            new Revista("Superinteressante", 480),
            new Midia("Central do Brasil", "DVD"),
        ];
        var datas = itens.Select(i => i.Emprestar(Retirada)).ToList();
        Assert.Equal(
            [new DateOnly(2026, 10, 20), new DateOnly(2026, 10, 12), new DateOnly(2026, 10, 8)],
            datas);
    }

    [Fact]
    public void ItemEmprestadoFicaIndisponivelERecusaSegundoEmprestimo()
    {
        var revista = new Revista("Superinteressante", 480);
        revista.Emprestar(Retirada);
        Assert.False(revista.Disponivel);
        var erro = Assert.Throws<InvalidOperationException>(() => revista.Emprestar(Retirada));
        Assert.Contains("Superinteressante", erro.Message);
    }

    [Fact]
    public void DevolverItemQueNaoSaiuLancaExcecao()
    {
        Assert.Throws<InvalidOperationException>(() => new Midia("Central do Brasil", "DVD").Devolver());
    }

    [Fact]
    public void TituloVazioERecusadoPelaClasseBase()
    {
        Assert.Throws<ArgumentException>(() => new Livro("", "Machado de Assis"));
    }
}
