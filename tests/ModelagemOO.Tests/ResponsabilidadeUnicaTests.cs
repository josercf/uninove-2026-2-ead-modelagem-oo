using Topico03;
using Topico04;
using Topico07;
using Topico11;
using Topico11.Responsabilidade;
using AntesSrp = Topico11.Responsabilidade.Antes;
using DepoisSrp = Topico11.Responsabilidade.Depois;

namespace ModelagemOO.Tests;

// Responsabilidade única: o mesmo cenário nos dois desenhos e a contagem de motivos de mudança por tipo.
public class ResponsabilidadeUnicaTests
{
    private static readonly DateOnly Retirada = new(2026, 11, 2);
    private static readonly DateOnly Prevista = new(2026, 11, 17);

    // Cenário comum: Joana (estudante) retira "Dom Casmurro" e devolve com 3 dias de atraso.
    private static (string Mensagem, ComprovanteDeDevolucao Comprovante, IReadOnlyList<string> Rastro) RodarAntes(Perfil perfil, int diasDeAtraso)
    {
        var balcao = new AntesSrp.BalcaoDeTudo();
        var joana = new Usuario("Joana", perfil);
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        livro.Emprestar(Retirada);
        var mensagem = balcao.Emprestar(joana, livro.Titulo);
        var comprovante = balcao.Devolver(joana, livro, Prevista, Prevista.AddDays(diasDeAtraso));
        return (mensagem, comprovante, balcao.Rastro);
    }

    private static (string Mensagem, ComprovanteDeDevolucao Comprovante, IReadOnlyList<string> Rastro) RodarDepois(Perfil perfil, int diasDeAtraso)
    {
        var rastro = new DepoisSrp.RastroDoBalcao();
        var balcao = new DepoisSrp.BalcaoDeAtendimento(new DepoisSrp.RegraDeEmprestimo(), new DepoisSrp.RegraDeMulta(), rastro);
        var joana = new Usuario("Joana", perfil);
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        livro.Emprestar(Retirada);
        var mensagem = balcao.Emprestar(joana, livro.Titulo);
        var comprovante = balcao.Devolver(joana, livro, Prevista, Prevista.AddDays(diasDeAtraso));
        return (mensagem, comprovante, rastro.Linhas);
    }

    [Theory]
    [InlineData(Perfil.Morador, 0, 0.00)]
    [InlineData(Perfil.Morador, 3, 3.00)]
    [InlineData(Perfil.Estudante, 3, 1.50)]
    [InlineData(Perfil.Voluntario, 3, 0.75)]
    [InlineData(Perfil.Estudante, 10, 5.00)]
    public void OsDoisDesenhosProduzemOMesmoResultado(Perfil perfil, int dias, double multaEsperada)
    {
        var antes = RodarAntes(perfil, dias);
        var depois = RodarDepois(perfil, dias);

        Assert.Equal("Empréstimo de \"Dom Casmurro\" registrado para Joana.", antes.Mensagem);
        Assert.Equal(antes.Mensagem, depois.Mensagem);
        Assert.Equal((decimal)multaEsperada, antes.Comprovante.Multa);
        Assert.Equal(antes.Comprovante, depois.Comprovante);
        Assert.Equal(antes.Rastro, depois.Rastro);
    }

    [Fact]
    public void RastroDoCenarioTemUmaLinhaPorEvento()
    {
        var depois = RodarDepois(Perfil.Estudante, 3);
        Assert.Equal(
            ["Empréstimo de Dom Casmurro para Joana: aceito", "Devolução de Dom Casmurro: 3 dia(s), multa R$ 1,50"],
            depois.Rastro);
    }

    [Fact]
    public void RecusaPorMultaEmAbertoTemAMesmaMensagemNosDoisDesenhos()
    {
        var antes = new AntesSrp.BalcaoDeTudo();
        var depois = new DepoisSrp.BalcaoDeAtendimento(new DepoisSrp.RegraDeEmprestimo(), new DepoisSrp.RegraDeMulta(), new DepoisSrp.RastroDoBalcao());
        var u1 = new Usuario("Joana", Perfil.Estudante);
        var u2 = new Usuario("Joana", Perfil.Estudante);
        u1.RegistrarMulta();
        u2.RegistrarMulta();

        var mensagemAntes = antes.Emprestar(u1, "Dom Casmurro");
        var mensagemDepois = depois.Emprestar(u2, "Dom Casmurro");

        Assert.Equal("Usuário com multa em aberto não retira item novo.", mensagemAntes);
        Assert.Equal(mensagemAntes, mensagemDepois);
        Assert.Empty(u1.Itens);
        Assert.Empty(u2.Itens);
    }

    [Fact]
    public void RecusaFicaNoRastroNosDoisDesenhos()
    {
        var antes = new AntesSrp.BalcaoDeTudo();
        var rastro = new DepoisSrp.RastroDoBalcao();
        var depois = new DepoisSrp.BalcaoDeAtendimento(new DepoisSrp.RegraDeEmprestimo(), new DepoisSrp.RegraDeMulta(), rastro);
        var u1 = new Usuario("Joana", Perfil.Morador);
        var u2 = new Usuario("Joana", Perfil.Morador);
        antes.Emprestar(u1, "A");
        antes.Emprestar(u1, "B");
        antes.Emprestar(u1, "C");
        depois.Emprestar(u2, "A");
        depois.Emprestar(u2, "B");
        depois.Emprestar(u2, "C");

        Assert.Equal(3, antes.Rastro.Count);
        Assert.EndsWith("recusado", antes.Rastro[2]);
        Assert.Equal(antes.Rastro, rastro.Linhas);
    }

    [Fact]
    public void DevolverNoPrazoNaoRegistraMultaNemMarcaOUsuario()
    {
        var joana = new Usuario("Joana", Perfil.Estudante);
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        livro.Emprestar(Retirada);
        var balcao = new DepoisSrp.BalcaoDeAtendimento(new DepoisSrp.RegraDeEmprestimo(), new DepoisSrp.RegraDeMulta(), new DepoisSrp.RastroDoBalcao());

        var comprovante = balcao.Devolver(joana, livro, Prevista, Prevista);

        Assert.Equal(0m, comprovante.Multa);
        Assert.False(joana.TemMultaEmAberto);
    }

    [Fact]
    public void DevolverComAtrasoMarcaMultaEmAbertoNoUsuarioNosDoisDesenhos()
    {
        var u1 = new Usuario("Joana", Perfil.Estudante);
        var l1 = new Livro("Dom Casmurro", "Machado de Assis");
        l1.Emprestar(Retirada);
        new AntesSrp.BalcaoDeTudo().Devolver(u1, l1, Prevista, Prevista.AddDays(1));

        var u2 = new Usuario("Joana", Perfil.Estudante);
        var l2 = new Livro("Dom Casmurro", "Machado de Assis");
        l2.Emprestar(Retirada);
        new DepoisSrp.BalcaoDeAtendimento(new DepoisSrp.RegraDeEmprestimo(), new DepoisSrp.RegraDeMulta(), new DepoisSrp.RastroDoBalcao())
            .Devolver(u2, l2, Prevista, Prevista.AddDays(1));

        Assert.True(u1.TemMultaEmAberto);
        Assert.True(u2.TemMultaEmAberto);
    }

    // ---- Motivos de mudança medidos por reflexão ----

    [Fact]
    public void BalcaoDeTudoTemQuatroMotivosDeMudancaNumTipoSo()
    {
        var motivos = MedidorDeMotivos.Motivos(typeof(AntesSrp.BalcaoDeTudo));
        Assert.Equal(
            new HashSet<string> { "regra de empréstimo", "regra de multa", "registro do rastro", "texto das mensagens" },
            motivos.ToHashSet());
    }

    [Fact]
    public void CadaTipoDoDesenhoDepoisTemUmMotivoSo()
    {
        foreach (var tipo in MedidorDeMotivos.TiposDoNamespace("Topico11.Responsabilidade.Depois"))
            Assert.True(MedidorDeMotivos.Contar(tipo) == 1, $"{tipo.Name} tem {MedidorDeMotivos.Contar(tipo)} motivos.");
    }

    [Fact]
    public void OsMotivosDoAntesFicamEspalhadosEmCincoTiposNoDepois()
    {
        var tipos = MedidorDeMotivos.TiposDoNamespace("Topico11.Responsabilidade.Depois").ToList();
        Assert.Equal(5, tipos.Count);
        var distintos = tipos.SelectMany(MedidorDeMotivos.Motivos).Distinct().Count();
        // Os quatro motivos do antes continuam existindo, mais a ordem do atendimento, que é do coordenador.
        Assert.Equal(5, distintos);
        Assert.Contains("ordem do atendimento", tipos.SelectMany(MedidorDeMotivos.Motivos));
    }

    [Fact]
    public void TodoMetodoDeclaradoNosDoisDesenhosCarregaMotivo()
    {
        var tipos = MedidorDeMotivos.TiposDoNamespace("Topico11.Responsabilidade.Antes")
            .Concat(MedidorDeMotivos.TiposDoNamespace("Topico11.Responsabilidade.Depois"));
        foreach (var tipo in tipos)
            Assert.Empty(MedidorDeMotivos.MetodosSemMotivo(tipo));
    }

    // Teste do próprio medidor, com tipos sintéticos.
    private class DoisMotivos
    {
        [MotivoDeMudanca("a")] public void X() { }
        [MotivoDeMudanca("b")] [MotivoDeMudanca("a")] public void Y() { }
        public void SemRotulo() { }
    }

    [Fact]
    public void MedidorContaMotivosDistintosEApontaMetodoSemMotivo()
    {
        Assert.Equal(2, MedidorDeMotivos.Contar(typeof(DoisMotivos)));
        Assert.Equal(["SemRotulo"], MedidorDeMotivos.MetodosSemMotivo(typeof(DoisMotivos)).ToArray());
    }

    // ---- Histórico por versões: o git guarda um commit por arquivo, então a medida é por pasta de tópico ----

    [Fact]
    public void HistoricoDoBalcaoPorVersoesDosTopicos()
    {
        Assert.Equal(1, MetodosPublicos(typeof(Topico05.Balcao)));
        Assert.Equal(0, ParametrosDoConstrutor(typeof(Topico05.Balcao)));

        Assert.Equal(2, MetodosPublicos(typeof(BalcaoComRastro)));
        Assert.Equal(2, ParametrosDoConstrutor(typeof(BalcaoComRastro)));

        Assert.Equal(1, MetodosPublicos(typeof(Topico09.Depois.Balcao)));
        Assert.Equal(1, ParametrosDoConstrutor(typeof(Topico09.Depois.Balcao)));

        Assert.Equal(2, MetodosPublicos(typeof(AntesSrp.BalcaoDeTudo)));
    }

    [Fact]
    public void MotivosPorVersaoSaoOsDaTabelaDoHistorico()
    {
        var historico = HistoricoDoBalcao.Versoes;
        Assert.Equal(
            [("topico-05", 2), ("topico-07", 4), ("topico-09 depois", 2), ("topico-11 antes", 4)],
            historico.Select(v => (v.Versao, v.Motivos.Count)).ToArray());
        // A última linha da tabela é medida, e não declarada: bate com a reflexão sobre BalcaoDeTudo.
        Assert.Equal(MedidorDeMotivos.Contar(typeof(AntesSrp.BalcaoDeTudo)), historico[^1].Motivos.Count);
    }

    private static int MetodosPublicos(Type t) =>
        t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
            .Count(m => !m.IsSpecialName);

    private static int ParametrosDoConstrutor(Type t) =>
        t.GetConstructors().Max(c => c.GetParameters().Length);
}
