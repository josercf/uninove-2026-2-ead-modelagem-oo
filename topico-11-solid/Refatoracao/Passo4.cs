using System.Globalization;
using Topico04;
using Topico10.Depois;

namespace Topico11.Refatoracao;

// Passo 4: a escolha do valor por perfil sai daqui e passa a usar o Strategy do tópico 10.
public static class Passo4
{
    public static string Resumir(Perfil perfil, DateOnly prevista, DateOnly entrega, string titulo)
    {
        var dias = DiasDeAtraso(prevista, entrega);
        if (dias == 0)
            return SemMulta(titulo);
        var multa = new CalculadoraDeMulta(PoliticasPorPerfil.Para(perfil)).Calcular(dias);
        return ComMulta(titulo, dias, multa);
    }

    private static int DiasDeAtraso(DateOnly prevista, DateOnly entrega) =>
        Math.Max(0, entrega.DayNumber - prevista.DayNumber);

    private static string SemMulta(string titulo) => $"{titulo}: devolvido no prazo, sem multa.";

    private static string ComMulta(string titulo, int dias, decimal multa) =>
        $"{titulo}: {dias} dia(s) de atraso, multa de R$ {multa.ToString("0.00", CultureInfo.GetCultureInfo("pt-BR"))}.";
}
