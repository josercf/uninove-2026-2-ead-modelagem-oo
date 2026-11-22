using Topico04;

namespace Topico10.Depois;

// Strategy: a interface fixa o contrato, e cada perfil traz a própria regra.
public interface IPoliticaDeMulta
{
    decimal Calcular(int diasDeAtraso); // sem corpo: cada política escreve a própria conta
}

public class MultaDoMorador : IPoliticaDeMulta // ": IPoliticaDeMulta" promete cumprir a interface
{
    public decimal Calcular(int diasDeAtraso) => diasDeAtraso * 1.00m; // valor fictício: R$ 1,00 por dia
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
    private readonly IPoliticaDeMulta _politica; // guarda a política sem saber qual classe ela é

    public CalculadoraDeMulta(IPoliticaDeMulta politica) // quem cria a calculadora escolhe a política
    {
        ArgumentNullException.ThrowIfNull(politica); // sem política não há cálculo
        _politica = politica; // a escolha fica fixa durante a vida da calculadora
    }

    public decimal Calcular(int diasDeAtraso)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(diasDeAtraso); // atraso negativo é recusado
        return _politica.Calcular(diasDeAtraso); // delega: a classe real da política responde
    }
}

// A escolha da política continua existindo: ela sai do cálculo e fica num ponto só.
public static class PoliticasPorPerfil
{
    public static IPoliticaDeMulta Para(Perfil perfil) => perfil switch // o switch sobre o perfil mudou para cá
    {
        Perfil.Morador => new MultaDoMorador(),
        Perfil.Estudante => new MultaDoEstudante(),
        Perfil.Voluntario => new MultaDoVoluntario(),
        _ => throw new ArgumentOutOfRangeException(nameof(perfil)),
    };
}
