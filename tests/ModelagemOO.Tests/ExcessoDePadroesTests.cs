using Topico10;
using Simples = Topico10.Excesso.Simples;
using ComPadroes = Topico10.Excesso.ComPadroes;

namespace ModelagemOO.Tests;

// Limite: o limite de renovações é uma constante, e três padrões em volta dela só somam classes e linhas.
public class ExcessoDePadroesTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(2, false)]
    [InlineData(5, false)]
    public void AsDuasVersoesDecidemIgual(int renovacoes, bool esperado)
    {
        Assert.Equal(esperado, Simples.Renovacao.Permitida(renovacoes));
        var controle = new ComPadroes.ControleDeRenovacao(ComPadroes.FabricaDePoliticaDeRenovacao.Criar());
        Assert.Equal(esperado, controle.Tentar(renovacoes));
    }

    [Fact]
    public void AVersaoComPadroesRegistraAsNegativasNoObservador()
    {
        var registro = new ComPadroes.RegistroDeRenovacoesNegadas();
        var controle = new ComPadroes.ControleDeRenovacao(ComPadroes.FabricaDePoliticaDeRenovacao.Criar());
        controle.Inscrever(registro);
        controle.Tentar(0);
        controle.Tentar(1);
        Assert.Equal([1], registro.RenovacoesNegadas);
    }

    [Fact]
    public void ContagemDeClassesEDeLinhasDasDuasVersoes()
    {
        var simples = MedidorDeTamanho.Medir("Topico10.Excesso.Simples", "topico-10-padroes-gof/Excesso/Simples");
        var comPadroes = MedidorDeTamanho.Medir("Topico10.Excesso.ComPadroes", "topico-10-padroes-gof/Excesso/ComPadroes");

        Assert.Equal(new Tamanho(1, 3), simples);
        Assert.Equal(new Tamanho(6, 20), comPadroes);
    }

    [Fact]
    public void ComPadroesTemMaisClassesEMaisLinhasQueASimples()
    {
        var simples = MedidorDeTamanho.Medir("Topico10.Excesso.Simples", "topico-10-padroes-gof/Excesso/Simples");
        var comPadroes = MedidorDeTamanho.Medir("Topico10.Excesso.ComPadroes", "topico-10-padroes-gof/Excesso/ComPadroes");
        Assert.True(comPadroes.Tipos > simples.Tipos);
        Assert.True(comPadroes.Linhas > simples.Linhas);
    }

    // Teste do próprio medidor, sobre pastas sintéticas.
    [Fact]
    public void LinhasIgnoramBrancasComentariosChavesIsoladasUsingENamespace()
    {
        var pasta = Directory.CreateTempSubdirectory().FullName;
        try
        {
            File.WriteAllText(Path.Combine(pasta, "A.cs"),
                "using System;\nnamespace X;\n\n// comentário\npublic class A\n{\n    public int V => 1; // fim\n}\n");
            Assert.Equal(2, MedidorDeTamanho.ContarLinhasDeCodigo(pasta));
        }
        finally
        {
            Directory.Delete(pasta, true);
        }
    }

    [Fact]
    public void RaizDoRepositorioContemASolucao()
    {
        Assert.True(File.Exists(Path.Combine(MedidorDeTamanho.RaizDoRepositorio(), "ModelagemOO.sln")));
    }
}
