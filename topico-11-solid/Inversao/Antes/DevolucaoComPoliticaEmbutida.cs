using Topico04;
using Topico10.Depois;

namespace Topico11.Inversao.Antes;

// Desenho antes: a devolução cria e escolhe as classes concretas de multa. Perfil novo exige editar esta classe.
public class DevolucaoComPoliticaEmbutida
{
    private readonly MultaDoMorador _morador = new();
    private readonly MultaDoEstudante _estudante = new();
    private readonly MultaDoVoluntario _voluntario = new();

    public ComprovanteDaMulta Devolver(Perfil perfil, string titulo, int diasDeAtraso)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(diasDeAtraso);
        var multa = perfil switch
        {
            Perfil.Morador => _morador.Calcular(diasDeAtraso),
            Perfil.Estudante => _estudante.Calcular(diasDeAtraso),
            Perfil.Voluntario => _voluntario.Calcular(diasDeAtraso),
            _ => throw new ArgumentOutOfRangeException(nameof(perfil)),
        };
        return new ComprovanteDaMulta(titulo, diasDeAtraso, multa);
    }
}
