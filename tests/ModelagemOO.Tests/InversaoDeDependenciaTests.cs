using Topico04;
using Topico10.Depois;
using Topico11;
using AntesInv = Topico11.Inversao.Antes;
using DepoisInv = Topico11.Inversao.Depois;

namespace ModelagemOO.Tests;

// Aberto-fechado e inversão de dependência, a partir do Strategy do tópico 10.
public class InversaoDeDependenciaTests
{
    // Política nova, escrita aqui no teste: isenta de multa os primeiros dois dias e cobra R$ 2,00 por dia depois.
    private class MultaComCarencia : IPoliticaDeMulta
    {
        public decimal Calcular(int diasDeAtraso) => Math.Max(0, diasDeAtraso - 2) * 2.00m;
    }

    public static TheoryData<Perfil, int, decimal> Cenario => new()
    {
        { Perfil.Morador, 0, 0.00m },
        { Perfil.Morador, 3, 3.00m },
        { Perfil.Estudante, 3, 1.50m },
        { Perfil.Voluntario, 3, 0.75m },
        { Perfil.Estudante, 10, 5.00m },
    };

    [Theory]
    [MemberData(nameof(Cenario))]
    public void OsDoisDesenhosCobramOMesmoValorParaOMesmoPerfil(Perfil perfil, int dias, decimal esperado)
    {
        var antes = new AntesInv.DevolucaoComPoliticaEmbutida().Devolver(perfil, "Dom Casmurro", dias);
        var depois = new DepoisInv.DevolucaoComPoliticaInjetada(PoliticasPorPerfil.Para(perfil)).Devolver("Dom Casmurro", dias);

        Assert.Equal(esperado, antes.Multa);
        Assert.Equal(antes, depois);
    }

    [Fact]
    public void PoliticaNovaEntraNoDesenhoDepoisSemEditarClasseExistente()
    {
        var devolucao = new DepoisInv.DevolucaoComPoliticaInjetada(new MultaComCarencia());

        Assert.Equal(0m, devolucao.Devolver("Dom Casmurro", 2).Multa);
        Assert.Equal(6.00m, devolucao.Devolver("Dom Casmurro", 5).Multa);
    }

    [Fact]
    public void DesenhoAntesFicaFechadoParaPerfilQueNaoEstaNoSwitch()
    {
        var antes = new AntesInv.DevolucaoComPoliticaEmbutida();
        Assert.Throws<ArgumentOutOfRangeException>(() => antes.Devolver((Perfil)99, "Dom Casmurro", 3));
    }

    [Fact]
    public void AtrasoNegativoEhRecusadoNosDoisDesenhos()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AntesInv.DevolucaoComPoliticaEmbutida().Devolver(Perfil.Morador, "X", -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DepoisInv.DevolucaoComPoliticaInjetada(new MultaDoMorador()).Devolver("X", -1));
    }

    [Fact]
    public void PoliticaNulaEhRecusadaNoConstrutor()
    {
        Assert.Throws<ArgumentNullException>(() => new DepoisInv.DevolucaoComPoliticaInjetada(null!));
    }

    // ---- Dependências medidas por reflexão ----

    [Fact]
    public void DesenhoAntesDependeDeTresClassesConcretas()
    {
        var d = MedidorDeDependencias.Medir(typeof(AntesInv.DevolucaoComPoliticaEmbutida));
        Assert.Equal(new Dependencias(Concretas: 3, Abstracoes: 0), d);
    }

    [Fact]
    public void DesenhoDepoisDependeDeUmaAbstracao()
    {
        var d = MedidorDeDependencias.Medir(typeof(DepoisInv.DevolucaoComPoliticaInjetada));
        Assert.Equal(new Dependencias(Concretas: 0, Abstracoes: 1), d);
    }

    // ---- Limite: o fechamento vale só para a variação que a interface previu ----

    [Fact]
    public void InterfaceDaPoliticaRecebeSoOsDiasDeAtraso()
    {
        var metodos = typeof(IPoliticaDeMulta).GetMethods();
        var calcular = Assert.Single(metodos);
        Assert.Equal([typeof(int)], calcular.GetParameters().Select(p => p.ParameterType).ToArray());
    }

    [Fact]
    public void PoliticaQuePrecisaDoTipoDoItemTemQueEsperarPelaMudancaDaInterface()
    {
        // Uma regra por tipo de item não cabe em Calcular(int): o dado não chega à política.
        // O caminho é ampliar a interface, e isso obriga a editar as três políticas existentes.
        var implementacoes = MedidorDeDependencias.Implementacoes(typeof(IPoliticaDeMulta));
        Assert.Equal(3, implementacoes.Count);
    }

    // Teste do próprio medidor, com tipos sintéticos.
    private interface IAbs { }
    private class Conc { }
    private class UsaTres
    {
        private readonly IAbs? _a = null;
        private readonly Conc _c = new();
        private readonly string _texto = "x";
        public UsaTres(IAbs a, Conc c) { _ = a; _ = c; _ = _a; _ = _c; _ = _texto; }
    }

    [Fact]
    public void MedidorIgnoraTiposDoSistemaEContaCampoEParametroUmaVezSo()
    {
        // O namespace dos tipos sintéticos é ModelagemOO.Tests, fora de Topico*, então o medidor não os vê.
        Assert.Equal(new Dependencias(0, 0), MedidorDeDependencias.Medir(typeof(UsaTres)));
        Assert.Equal(new Dependencias(1, 1), MedidorDeDependencias.Medir(typeof(UsaTres), prefixoDoNamespace: "ModelagemOO"));
    }
}
