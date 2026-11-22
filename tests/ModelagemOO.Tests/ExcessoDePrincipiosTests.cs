using Topico10;
using Topico10.Depois;
using Topico11;
using Simples = Topico11.Excesso.Simples;
using ComPrincipios = Topico11.Excesso.ComPrincipios;

namespace ModelagemOO.Tests;

// Limite: a mensagem de confirmação do empréstimo tem um texto só, e quatro princípios em volta dela só somam tipos e linhas.
public class ExcessoDePrincipiosTests
{
    [Theory]
    [InlineData("Dom Casmurro", "Joana")]
    [InlineData("Piauí", "Marcos")]
    public void AsDuasVersoesProduzemOMesmoTexto(string titulo, string nome)
    {
        var esperado = $"Empréstimo de \"{titulo}\" registrado para {nome}.";
        Assert.Equal(esperado, Simples.Confirmacao.Emprestimo(titulo, nome));

        var entregador = new ComPrincipios.EntregadorEmMemoria();
        var servico = ComPrincipios.FabricaDeServicoDeConfirmacao.Criar(entregador);
        Assert.Equal(esperado, servico.Confirmar(titulo, nome));
        Assert.Equal([esperado], entregador.Entregues);
    }

    [Fact]
    public void AVersaoComPrincipiosAceitaFormatadorEEntregadorDeFora()
    {
        var entregador = new ComPrincipios.EntregadorEmMemoria();
        var servico = new ComPrincipios.ServicoDeConfirmacao(new FormatadorEmIngles(), entregador);
        Assert.Equal("Loan of \"Dom Casmurro\" registered for Joana.", servico.Confirmar("Dom Casmurro", "Joana"));
    }

    private class FormatadorEmIngles : ComPrincipios.IFormatadorDeConfirmacao
    {
        public string Formatar(string titulo, string nome) => $"Loan of \"{titulo}\" registered for {nome}.";
    }

    [Fact]
    public void ContagemDeTiposEDeLinhasDasDuasVersoes()
    {
        var simples = MedidorDeTamanhoDoTopico.Medir("Topico11.Excesso.Simples", "topico-11-solid/Excesso/Simples");
        var comPrincipios = MedidorDeTamanhoDoTopico.Medir("Topico11.Excesso.ComPrincipios", "topico-11-solid/Excesso/ComPrincipios");

        Assert.Equal(new Tamanho(1, 2), simples);
        Assert.Equal(new Tamanho(6, 16), comPrincipios);
    }

    [Fact]
    public void ComPrincipiosTemMaisTiposEMaisLinhasQueASimples()
    {
        var simples = MedidorDeTamanhoDoTopico.Medir("Topico11.Excesso.Simples", "topico-11-solid/Excesso/Simples");
        var comPrincipios = MedidorDeTamanhoDoTopico.Medir("Topico11.Excesso.ComPrincipios", "topico-11-solid/Excesso/ComPrincipios");
        Assert.True(comPrincipios.Tipos > simples.Tipos);
        Assert.True(comPrincipios.Linhas > simples.Linhas);
    }

    // ---- A força que falta: nenhuma das duas interfaces tem segunda implementação ----

    [Fact]
    public void CadaInterfaceDaVersaoComPrincipiosTemUmaImplementacaoSo()
    {
        Assert.Single(MedidorDeDependencias.Implementacoes(typeof(ComPrincipios.IFormatadorDeConfirmacao))); // Assert.Single reprova se a lista não tiver exatamente um elemento
        Assert.Single(MedidorDeDependencias.Implementacoes(typeof(ComPrincipios.IEntregadorDeConfirmacao)));
    }

    [Fact]
    public void AInterfaceDaPoliticaDeMultaTemTresImplementacoes()
    {
        // Comparação: no tópico 10, a variação por perfil existe, e a interface tem três implementações.
        Assert.Equal(3, MedidorDeDependencias.Implementacoes(typeof(IPoliticaDeMulta)).Count);
    }

    // Teste do próprio medidor, sobre uma pasta sintética: o tipo contado é o do namespace, não o aninhado.
    [Fact]
    public void MedidorDoTopicoContaLinhasDePastaSintetica()
    {
        var pasta = Directory.CreateTempSubdirectory().FullName;
        try
        {
            File.WriteAllText(Path.Combine(pasta, "A.cs"), "using System;\nnamespace X;\n// nota\npublic class A\n{\n    public int V => 1;\n}\n");
            Assert.Equal(2, MedidorDeTamanho.ContarLinhasDeCodigo(pasta));
        }
        finally
        {
            Directory.Delete(pasta, true);
        }
    }

    [Fact]
    public void MedidorDoTopicoNaoContaTiposDeOutroNamespace()
    {
        Assert.Equal(0, MedidorDeTamanhoDoTopico.ContarTipos("Topico11.Nao.Existe"));
    }
}
