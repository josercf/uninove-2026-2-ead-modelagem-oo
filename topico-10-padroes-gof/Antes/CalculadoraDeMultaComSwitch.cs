using Topico04;

namespace Topico10.Antes;

// Desenho antes: a regra de cada perfil mora num switch dentro de um único método.
public static class CalculadoraDeMultaComSwitch
{
    public static decimal Calcular(Perfil perfil, int diasDeAtraso)
    {
        // Atraso negativo pagaria o usuário por devolver cedo: é recusado.
        ArgumentOutOfRangeException.ThrowIfNegative(diasDeAtraso);
        // Valores de exemplo desta aula: multa por dia de atraso em reais.
        return perfil switch
        {
            Perfil.Morador => diasDeAtraso * 1.00m,
            Perfil.Estudante => diasDeAtraso * 0.50m,
            Perfil.Voluntario => diasDeAtraso * 0.25m,
            _ => throw new ArgumentOutOfRangeException(nameof(perfil)),
        };
    }
}
