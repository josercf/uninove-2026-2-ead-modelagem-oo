using Topico04;
using Topico10.Antes;
using Topico10.Depois;

namespace ModelagemOO.Tests;

// Mesmo cenário nos dois desenhos: o valor da multa de cada perfil para cada atraso.
public class StrategyMultaTests
{
    public static TheoryData<Perfil, int, decimal> Cenario => new()
    {
        { Perfil.Morador, 0, 0.00m },
        { Perfil.Morador, 1, 1.00m },
        { Perfil.Morador, 10, 10.00m },
        { Perfil.Estudante, 0, 0.00m },
        { Perfil.Estudante, 1, 0.50m },
        { Perfil.Estudante, 10, 5.00m },
        { Perfil.Voluntario, 0, 0.00m },
        { Perfil.Voluntario, 1, 0.25m },
        { Perfil.Voluntario, 10, 2.50m },
    };

    [Theory]
    [MemberData(nameof(Cenario))]
    public void DesenhoAntesCalculaAMultaDoPerfil(Perfil perfil, int dias, decimal esperado)
    {
        Assert.Equal(esperado, CalculadoraDeMultaComSwitch.Calcular(perfil, dias));
    }

    [Theory]
    [MemberData(nameof(Cenario))]
    public void DesenhoDepoisCalculaAMultaDoPerfil(Perfil perfil, int dias, decimal esperado)
    {
        var calculadora = new CalculadoraDeMulta(PoliticasPorPerfil.Para(perfil));
        Assert.Equal(esperado, calculadora.Calcular(dias));
    }

    [Theory]
    [InlineData(Perfil.Morador)]
    [InlineData(Perfil.Estudante)]
    [InlineData(Perfil.Voluntario)]
    public void AtrasoNegativoERecusadoNosDoisDesenhos(Perfil perfil)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CalculadoraDeMultaComSwitch.Calcular(perfil, -1));
        var calculadora = new CalculadoraDeMulta(PoliticasPorPerfil.Para(perfil));
        Assert.Throws<ArgumentOutOfRangeException>(() => calculadora.Calcular(-1));
    }

    [Fact]
    public void PerfilForaDaListaERecusadoNosDoisDesenhos()
    {
        var invalido = (Perfil)99;
        Assert.Throws<ArgumentOutOfRangeException>(() => CalculadoraDeMultaComSwitch.Calcular(invalido, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => PoliticasPorPerfil.Para(invalido));
    }

    // Ponto de extensão: uma política nova entra sem alterar a CalculadoraDeMulta.
    private sealed class MultaDoIdoso : IPoliticaDeMulta
    {
        public decimal Calcular(int diasDeAtraso) => 0m;
    }

    [Fact]
    public void PoliticaNovaEntraSemAlterarACalculadora()
    {
        var calculadora = new CalculadoraDeMulta(new MultaDoIdoso());
        Assert.Equal(0m, calculadora.Calcular(30));
    }

    [Fact]
    public void CalculadoraExigePolitica()
    {
        Assert.Throws<ArgumentNullException>(() => new CalculadoraDeMulta(null!));
    }
}
