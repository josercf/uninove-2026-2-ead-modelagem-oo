// Testes do tópico 09: acoplamento medido por reflexão antes e depois da redistribuição de responsabilidades.
using BalcaoAntes = Topico09.Antes.Balcao;
using BalcaoDepois = Topico09.Depois.Balcao;
using Topico03;
using Topico04;
using Topico06;
using Topico09;
using Topico09.Antes;
using Topico09.Depois;

public class AcoplamentoTests
{
    // ---- Tipos sintéticos para testar o próprio medidor ----

    private class Alvo { }
    private class DoCampo { }
    private class DoParametro { }
    private class DoRetorno { }
    private class DoCorpo { }
    private class DaLista { }
    private class DeFora { }

    private class Exemplo
    {
        private DoCampo? _campo;
        public IReadOnlyList<DaLista>? Lista { get; set; }
        public DoRetorno Metodo(DoParametro p) { _ = _campo; return new DoRetorno(); }
        public void SoNoCorpo() { var x = new DoCorpo(); GC.KeepAlive(x); GC.KeepAlive(new DeFora()); }
        public void SemDependencia(Exemplo outro) { GC.KeepAlive(outro); }
    }

    private static readonly Type[] UniversoSintetico =
        [typeof(Exemplo), typeof(DoCampo), typeof(DoParametro), typeof(DoRetorno), typeof(DoCorpo), typeof(DaLista), typeof(Alvo)];

    [Fact]
    public void MedidorEnxergaCampoParametroRetornoCorpoEArgumentoGenerico()
    {
        var deps = MedidorDeAcoplamento.Dependencias(typeof(Exemplo), UniversoSintetico);
        Assert.True(new HashSet<Type> { typeof(DoCampo), typeof(DoParametro), typeof(DoRetorno), typeof(DoCorpo), typeof(DaLista) }.SetEquals(deps));
    }

    [Fact]
    public void MedidorIgnoraTipoForaDoUniversoEAAutoReferencia()
    {
        var deps = MedidorDeAcoplamento.Dependencias(typeof(Exemplo), UniversoSintetico);
        Assert.DoesNotContain(typeof(DeFora), deps);
        Assert.DoesNotContain(typeof(Exemplo), deps);
        Assert.DoesNotContain(typeof(Alvo), deps);
    }

    [Fact]
    public void MedidorListaQuemCriaUmTipoComNew()
    {
        var criadores = MedidorDeAcoplamento.Criadores(typeof(DoCorpo), UniversoSintetico);
        Assert.Equal([typeof(Exemplo)], criadores);
        Assert.Empty(MedidorDeAcoplamento.Criadores(typeof(Alvo), UniversoSintetico));
    }

    // ---- Os dois desenhos do mesmo caso de uso ----

    private static readonly Type[] UniversoAntes =
        [typeof(BalcaoAntes), typeof(UsuarioAnemico), typeof(ItemAnemico), typeof(EmprestimoAnemico),
         typeof(TipoDeItem), typeof(Perfil), typeof(Topico05.ResultadoAtendimento)];

    private static readonly Type[] UniversoDepois =
        [typeof(BalcaoDepois), typeof(RegistroDeEmprestimos), typeof(Usuario), typeof(ItemAcervo), typeof(Livro),
         typeof(Revista), typeof(Midia), typeof(Emprestimo), typeof(Perfil), typeof(Topico05.ResultadoAtendimento)];

    [Fact]
    public void BalcaoAntesDependeDeSeisTipos()
    {
        var deps = MedidorDeAcoplamento.Dependencias(typeof(BalcaoAntes), UniversoAntes);
        Assert.True(new HashSet<Type> { typeof(UsuarioAnemico), typeof(ItemAnemico), typeof(EmprestimoAnemico),
            typeof(TipoDeItem), typeof(Perfil), typeof(Topico05.ResultadoAtendimento) }.SetEquals(deps));
    }

    [Fact]
    public void BalcaoDepoisDependeDeQuatroTipos()
    {
        var deps = MedidorDeAcoplamento.Dependencias(typeof(BalcaoDepois), UniversoDepois);
        Assert.True(new HashSet<Type> { typeof(RegistroDeEmprestimos), typeof(Usuario), typeof(ItemAcervo),
            typeof(Topico05.ResultadoAtendimento) }.SetEquals(deps));
    }

    [Fact]
    public void RedistribuicaoReduzOAcoplamentoDoBalcao()
    {
        var antes = MedidorDeAcoplamento.Dependencias(typeof(BalcaoAntes), UniversoAntes).Count;
        var depois = MedidorDeAcoplamento.Dependencias(typeof(BalcaoDepois), UniversoDepois).Count;
        Assert.Equal(6, antes);
        Assert.Equal(4, depois);
    }

    [Fact]
    public void RedistribuicaoReduzOsMembrosAlheiosQueOBalcaoUsa()
    {
        var antes = MedidorDeAcoplamento.MembrosAlheios(typeof(BalcaoAntes), UniversoAntes);
        var depois = MedidorDeAcoplamento.MembrosAlheios(typeof(BalcaoDepois), UniversoDepois);
        Assert.Equal(14, antes.Count);
        Assert.Equal(4, depois.Count);
    }

    [Fact]
    public void AcoplamentoTotalDoDesenhoCresceComAsClassesNovas()
    {
        // O total soma as dependências de todas as classes do universo. Mais classes, mais setas.
        Assert.Equal(10, MedidorDeAcoplamento.Total(UniversoAntes));
        Assert.Equal(13, MedidorDeAcoplamento.Total(UniversoDepois));
    }

    [Fact]
    public void NenhumaClasseDoDesenhoNovoPassaDeQuatroDependencias()
    {
        foreach (var tipo in UniversoDepois)
            Assert.True(MedidorDeAcoplamento.Dependencias(tipo, UniversoDepois).Count <= 4, tipo.Name);
    }

    // ---- GRASP Especialista ----

    [Theory]
    [InlineData("get_MultaEmAberto")]
    [InlineData("get_Perfil")]
    [InlineData("get_Itens")]
    [InlineData("get_Disponivel")]
    [InlineData("get_Tipo")]
    public void NoDesenhoAntesOBalcaoLeOsDadosQueDeviamFicarComOEspecialista(string membro)
    {
        var nomes = MedidorDeAcoplamento.MembrosAlheios(typeof(BalcaoAntes), UniversoAntes).Select(m => m.Name);
        Assert.Contains(membro, nomes);
    }

    [Theory]
    [InlineData("get_TemMultaEmAberto")]
    [InlineData("get_Limite")]
    [InlineData("get_Itens")]
    [InlineData("get_PrazoEmDias")]
    [InlineData("get_Disponivel")]
    [InlineData("Retirar")]
    [InlineData("Emprestar")]
    public void NoDesenhoDepoisOBalcaoNaoTocaNosMembrosDoEspecialista(string membro)
    {
        var nomes = MedidorDeAcoplamento.MembrosAlheios(typeof(BalcaoDepois), UniversoDepois).Select(m => m.Name);
        Assert.DoesNotContain(membro, nomes);
    }

    [Fact]
    public void AsRegrasDeLimiteMultaEPrazoVivemNasClassesQueTemOsDados()
    {
        // Usuario decide multa e limite; ItemAcervo decide prazo e disponibilidade.
        var usuario = MedidorDeAcoplamento.MembrosAlheios(typeof(Usuario), UniversoDepois);
        Assert.Empty(usuario);
        var item = MedidorDeAcoplamento.MembrosAlheios(typeof(ItemAcervo), UniversoDepois);
        Assert.Empty(item);
    }

    // ---- GRASP Criador ----

    [Fact]
    public void NoDesenhoDepoisSoORegistroCriaEmprestimo()
    {
        var criadores = MedidorDeAcoplamento.Criadores(typeof(Emprestimo), UniversoDepois);
        Assert.Equal([typeof(RegistroDeEmprestimos)], criadores);
    }

    [Fact]
    public void NoDesenhoAntesOBalcaoCriaOEmprestimo()
    {
        var criadores = MedidorDeAcoplamento.Criadores(typeof(EmprestimoAnemico), UniversoAntes);
        Assert.Equal([typeof(BalcaoAntes)], criadores);
    }

    // ---- Comportamento preservado ----

    private static (Topico05.ResultadoAtendimento Antes, Topico05.ResultadoAtendimento Depois) Atender(
        Perfil perfil, bool comMulta, int itensJaRetirados, bool itemEmprestado)
    {
        var data = new DateOnly(2026, 10, 5);

        var ua = new UsuarioAnemico("Joana", perfil) { MultaEmAberto = comMulta };
        for (var i = 0; i < itensJaRetirados; i++) ua.Itens.Add($"antigo {i}");
        var ia = new ItemAnemico("Dom Casmurro", TipoDeItem.Livro) { Disponivel = !itemEmprestado };
        var antes = new BalcaoAntes().Emprestar(ua, ia, data);

        var ud = new Usuario("Joana", perfil);
        if (comMulta) ud.RegistrarMulta();
        for (var i = 0; i < itensJaRetirados; i++) ud.Retirar($"antigo {i}");
        var id = new Livro("Dom Casmurro", "Machado de Assis");
        if (itemEmprestado) id.Emprestar(data);
        var depois = new BalcaoDepois(new RegistroDeEmprestimos()).Emprestar(ud, id, data);

        return (antes, depois);
    }

    [Theory]
    [InlineData(Perfil.Morador, false, 0, false, true)]
    [InlineData(Perfil.Morador, false, 2, false, false)]
    [InlineData(Perfil.Estudante, false, 3, false, true)]
    [InlineData(Perfil.Voluntario, false, 6, false, false)]
    [InlineData(Perfil.Estudante, true, 0, false, false)]
    [InlineData(Perfil.Morador, false, 0, true, false)]
    public void OsDoisDesenhosDecidemIgual(Perfil perfil, bool multa, int itens, bool emprestado, bool aceito)
    {
        var (antes, depois) = Atender(perfil, multa, itens, emprestado);
        Assert.Equal(aceito, antes.Aceito);
        Assert.Equal(antes, depois);
    }

    [Fact]
    public void ORegistroGuardaOEmprestimoDoDesenhoNovoComADataDeDevolucaoDoTipoDoItem()
    {
        var registro = new RegistroDeEmprestimos();
        var balcao = new BalcaoDepois(registro);
        balcao.Emprestar(new Usuario("Joana", Perfil.Morador), new Revista("Superinteressante", 480), new DateOnly(2026, 10, 5));
        var emprestimo = Assert.Single(registro.Emprestimos);
        Assert.Equal(new DateOnly(2026, 10, 12), emprestimo.Devolucao);
    }

    [Fact]
    public void RecusaNaoDeixaEmprestimoNoRegistro()
    {
        var registro = new RegistroDeEmprestimos();
        var usuario = new Usuario("Caio", Perfil.Estudante);
        usuario.RegistrarMulta();
        var resultado = new BalcaoDepois(registro).Emprestar(usuario, new Livro("Vidas Secas", "Graciliano Ramos"), new DateOnly(2026, 10, 5));
        Assert.False(resultado.Aceito);
        Assert.Empty(registro.Emprestimos);
    }

    [Fact]
    public void RecusaNoDesenhoAntesTambemNaoGuardaEmprestimo()
    {
        var balcao = new BalcaoAntes();
        var u = new UsuarioAnemico("Caio", Perfil.Estudante) { MultaEmAberto = true };
        var r = balcao.Emprestar(u, new ItemAnemico("Vidas Secas", TipoDeItem.Livro), new DateOnly(2026, 10, 5));
        Assert.False(r.Aceito);
        Assert.Empty(balcao.Emprestimos);
    }
}
