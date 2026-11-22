using Topico04;

namespace Topico11.Responsabilidade.Depois;

// Única razão de mudança: a regra que aceita ou recusa a retirada (RF01 e RF02).
public class RegraDeEmprestimo
{
    // Devolve a mensagem de recusa, ou nulo quando a retirada foi aceita.
    [MotivoDeMudanca("regra de empréstimo")]
    public string? Aplicar(Usuario usuario, string titulo)
    {
        try
        {
            usuario.Retirar(titulo);
            return null;
        }
        catch (InvalidOperationException recusa)
        {
            return recusa.Message;
        }
    }
}
