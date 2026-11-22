using System.Globalization;
using Topico04;

namespace Topico11.Refatoracao;

// Passo 1: Extract Method. O cálculo dos dias de atraso sai para um método com nome.
public static class Passo1
{
    public static string Resumir(Perfil perfil, DateOnly prevista, DateOnly entrega, string titulo)
    {
        var dias = DiasDeAtraso(prevista, entrega);
        if (dias == 0)
            return titulo + ": devolvido no prazo, sem multa.";
        decimal multa;
        if (perfil == Perfil.Morador)
            multa = dias * 1.00m;
        else if (perfil == Perfil.Estudante)
            multa = dias * 0.50m;
        else if (perfil == Perfil.Voluntario)
            multa = dias * 0.25m;
        else
            throw new ArgumentOutOfRangeException(nameof(perfil));
        return titulo + ": " + dias + " dia(s) de atraso, multa de R$ " + multa.ToString("0.00", CultureInfo.GetCultureInfo("pt-BR")) + ".";
    }

    private static int DiasDeAtraso(DateOnly prevista, DateOnly entrega) =>
        Math.Max(0, entrega.DayNumber - prevista.DayNumber);
}
