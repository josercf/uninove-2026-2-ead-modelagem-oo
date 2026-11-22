using System.Globalization;
using Topico04;

namespace Topico11.Refatoracao;

// Passo 0: o método original, com o cálculo dos dias, a escolha da multa e o texto no mesmo corpo.
public static class Passo0
{
    public static string Resumir(Perfil perfil, DateOnly prevista, DateOnly entrega, string titulo)
    {
        var dias = Math.Max(0, entrega.DayNumber - prevista.DayNumber);
        if (dias == 0)
            return titulo + ": devolvido no prazo, sem multa.";
        decimal multa;
        if (perfil == Perfil.Morador)
            multa = dias * 1.00m; // o sufixo m marca um valor decimal, o tipo usado para dinheiro
        else if (perfil == Perfil.Estudante)
            multa = dias * 0.50m;
        else if (perfil == Perfil.Voluntario)
            multa = dias * 0.25m;
        else
            throw new ArgumentOutOfRangeException(nameof(perfil));
        return titulo + ": " + dias + " dia(s) de atraso, multa de R$ " + multa.ToString("0.00", CultureInfo.GetCultureInfo("pt-BR")) + ".";
    }
}
