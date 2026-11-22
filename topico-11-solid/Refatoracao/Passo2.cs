using System.Globalization;
using Topico04;

namespace Topico11.Refatoracao;

// Passo 2: Extract Method e Replace Magic Number. A cadeia de if vira um switch com valores nomeados.
public static class Passo2
{
    private const decimal ValorDoMorador = 1.00m;
    private const decimal ValorDoEstudante = 0.50m;
    private const decimal ValorDoVoluntario = 0.25m;

    public static string Resumir(Perfil perfil, DateOnly prevista, DateOnly entrega, string titulo)
    {
        var dias = DiasDeAtraso(prevista, entrega);
        if (dias == 0)
            return titulo + ": devolvido no prazo, sem multa.";
        var multa = dias * ValorPorDia(perfil);
        return titulo + ": " + dias + " dia(s) de atraso, multa de R$ " + multa.ToString("0.00", CultureInfo.GetCultureInfo("pt-BR")) + ".";
    }

    private static int DiasDeAtraso(DateOnly prevista, DateOnly entrega) =>
        Math.Max(0, entrega.DayNumber - prevista.DayNumber);

    private static decimal ValorPorDia(Perfil perfil) => perfil switch
    {
        Perfil.Morador => ValorDoMorador,
        Perfil.Estudante => ValorDoEstudante,
        Perfil.Voluntario => ValorDoVoluntario,
        _ => throw new ArgumentOutOfRangeException(nameof(perfil)),
    };
}
