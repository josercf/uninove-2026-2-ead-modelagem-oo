using Topico04;
using Topico10.Depois;

namespace Topico11.Responsabilidade.Depois;

// Única razão de mudança: o valor da multa. A regra por perfil vem do Strategy do tópico 10.
public class RegraDeMulta
{
    [MotivoDeMudanca("regra de multa")]
    public decimal Calcular(Perfil perfil, int diasDeAtraso) =>
        new CalculadoraDeMulta(PoliticasPorPerfil.Para(perfil)).Calcular(diasDeAtraso);
}
