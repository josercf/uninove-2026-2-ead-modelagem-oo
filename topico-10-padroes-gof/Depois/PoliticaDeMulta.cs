using Topico04;

namespace Topico10.Depois;

// Strategy: a interface fixa o contrato, e cada perfil traz a própria regra.
public interface IPoliticaDeMulta
{
    decimal Calcular(int diasDeAtraso);
}

public class MultaDoMorador : IPoliticaDeMulta
{
    public decimal Calcular(int diasDeAtraso) => diasDeAtraso * 1.00m;
}

public class MultaDoEstudante : IPoliticaDeMulta
{
    public decimal Calcular(int diasDeAtraso) => diasDeAtraso * 0.50m;
}

public class MultaDoVoluntario : IPoliticaDeMulta
{
    public decimal Calcular(int diasDeAtraso) => diasDeAtraso * 0.25m;
}

// Contexto: recebe a política pronta e não sabe qual é.
public class CalculadoraDeMulta
{
    private readonly IPoliticaDeMulta _politica;

    public CalculadoraDeMulta(IPoliticaDeMulta politica)
    {
        ArgumentNullException.ThrowIfNull(politica);
        _politica = politica;
    }

    public decimal Calcular(int diasDeAtraso)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(diasDeAtraso);
        return _politica.Calcular(diasDeAtraso);
    }
}

// A escolha da política continua existindo: ela sai do cálculo e fica num ponto só.
public static class PoliticasPorPerfil
{
    public static IPoliticaDeMulta Para(Perfil perfil) => perfil switch
    {
        Perfil.Morador => new MultaDoMorador(),
        Perfil.Estudante => new MultaDoEstudante(),
        Perfil.Voluntario => new MultaDoVoluntario(),
        _ => throw new ArgumentOutOfRangeException(nameof(perfil)),
    };
}
