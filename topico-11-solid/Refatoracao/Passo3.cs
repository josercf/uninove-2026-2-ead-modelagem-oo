using System.Globalization;
using Topico04;

namespace Topico11.Refatoracao;

// Passo 3: Extract Method no texto. O corpo de Resumir passa a mostrar só a ordem das decisões.
public static class Passo3
{
    private const decimal ValorDoMorador = 1.00m;
    private const decimal ValorDoEstudante = 0.50m;
    private const decimal ValorDoVoluntario = 0.25m;

    public static string Resumir(Perfil perfil, DateOnly prevista, DateOnly entrega, string titulo)
    {
        var dias = DiasDeAtraso(prevista, entrega);
        if (dias == 0)
            return SemMulta(titulo);
        return ComMulta(titulo, dias, dias * ValorPorDia(perfil));
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

    private static string SemMulta(string titulo) => $"{titulo}: devolvido no prazo, sem multa.";

    private static string ComMulta(string titulo, int dias, decimal multa) =>
        $"{titulo}: {dias} dia(s) de atraso, multa de R$ {multa.ToString("0.00", CultureInfo.GetCultureInfo("pt-BR"))}.";
}
