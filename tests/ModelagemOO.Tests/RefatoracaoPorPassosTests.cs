using Topico04;
using Topico10;
using Topico11.Refatoracao;

namespace ModelagemOO.Tests;

// Refatoração guiada por teste: este arquivo é o mesmo antes e depois de cada passo.
// Cada passo é uma versão do mesmo método, e todas respondem a todos os casos abaixo.
public class RefatoracaoPorPassosTests
{
    private static readonly DateOnly Prevista = new(2026, 11, 17);

    private static readonly (Perfil Perfil, int Atraso, string Esperado)[] Casos =
    [
        (Perfil.Morador, -2, "Dom Casmurro: devolvido no prazo, sem multa."),
        (Perfil.Morador, 0, "Dom Casmurro: devolvido no prazo, sem multa."),
        (Perfil.Morador, 1, "Dom Casmurro: 1 dia(s) de atraso, multa de R$ 1,00."),
        (Perfil.Estudante, 3, "Dom Casmurro: 3 dia(s) de atraso, multa de R$ 1,50."),
        (Perfil.Voluntario, 3, "Dom Casmurro: 3 dia(s) de atraso, multa de R$ 0,75."),
        (Perfil.Voluntario, 10, "Dom Casmurro: 10 dia(s) de atraso, multa de R$ 2,50."),
        ((Perfil)99, 0, "Dom Casmurro: devolvido no prazo, sem multa."),
    ];

    public static TheoryData<string, Perfil, int, string> PassoPorCaso()
    {
        var dados = new TheoryData<string, Perfil, int, string>();
        foreach (var passo in Passos.Nomes)
            foreach (var (perfil, atraso, esperado) in Casos)
                dados.Add(passo, perfil, atraso, esperado);
        return dados;
    }

    public static TheoryData<string> NomesDosPassos()
    {
        var dados = new TheoryData<string>();
        foreach (var passo in Passos.Nomes)
            dados.Add(passo);
        return dados;
    }

    [Theory]
    [MemberData(nameof(PassoPorCaso))]
    public void CadaPassoDevolveOMesmoResumo(string passo, Perfil perfil, int atraso, string esperado)
    {
        var resumo = Passos.Resumir(passo, perfil, Prevista, Prevista.AddDays(atraso), "Dom Casmurro");
        Assert.Equal(esperado, resumo);
    }

    [Theory]
    [MemberData(nameof(NomesDosPassos))]
    public void PerfilForaDaListaComAtrasoEhRecusadoEmTodosOsPassos(string passo)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Passos.Resumir(passo, (Perfil)99, Prevista, Prevista.AddDays(3), "Dom Casmurro"));
    }

    [Fact]
    public void SaoCincoPassosNaOrdemDaRefatoracao()
    {
        Assert.Equal(["Passo0", "Passo1", "Passo2", "Passo3", "Passo4"], Passos.Nomes.ToArray());
    }

    [Fact]
    public void PassoDesconhecidoEhRecusado()
    {
        Assert.Throws<ArgumentException>(() => Passos.Resumir("Passo9", Perfil.Morador, Prevista, Prevista, "X"));
    }

    // ---- O que cada passo faz com o tamanho do método, medido pelo texto do arquivo ----

    [Fact]
    public void TamanhoDeCadaPassoEmMetodosELinhasComCodigo()
    {
        var medidas = Passos.Nomes.Select(n => (n, Passos.Medir(n))).ToArray();
        Assert.Equal(
            [("Passo0", new MedidaDoPasso(1, 15)), ("Passo1", new MedidaDoPasso(2, 17)),
             ("Passo2", new MedidaDoPasso(3, 18)), ("Passo3", new MedidaDoPasso(5, 20)),
             ("Passo4", new MedidaDoPasso(4, 12))],
            medidas);
    }

    [Fact]
    public void OMetodoPrincipalFicaMenorQueOOriginalDepoisDoPasso2()
    {
        var principal = Passos.Nomes.Select(n => Passos.LinhasDoMetodoPrincipal(n)).ToArray();
        Assert.Equal([14, 14, 6, 5, 6], principal);
    }

    // Teste do próprio contador de linhas, sobre uma pasta sintética.
    [Fact]
    public void ContadorDeLinhasDoMetodoIgnoraBrancasEComentarios()
    {
        var pasta = Directory.CreateTempSubdirectory().FullName;
        try
        {
            var arquivo = Path.Combine(pasta, "P.cs");
            File.WriteAllText(arquivo, "class P\n{\n    public static string Resumir(int a)\n    {\n        // nota\n\n        var x = a;\n        return x.ToString();\n    }\n\n    private static int Outro() => 1;\n}\n");
            Assert.Equal(3, Passos.LinhasDoMetodoNoArquivo(arquivo, "Resumir"));
        }
        finally
        {
            Directory.Delete(pasta, true);
        }
    }
}
