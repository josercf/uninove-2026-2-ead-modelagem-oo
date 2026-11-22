using Topico10.Depois;

namespace Topico11.Inversao.Depois;

// Desenho depois: a devolução depende da interface IPoliticaDeMulta e recebe a política já escolhida.
public class DevolucaoComPoliticaInjetada
{
    private readonly IPoliticaDeMulta _politica;

    public DevolucaoComPoliticaInjetada(IPoliticaDeMulta politica)
    {
        ArgumentNullException.ThrowIfNull(politica);
        _politica = politica;
    }

    public ComprovanteDaMulta Devolver(string titulo, int diasDeAtraso)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(diasDeAtraso);
        return new ComprovanteDaMulta(titulo, diasDeAtraso, _politica.Calcular(diasDeAtraso));
    }
}
