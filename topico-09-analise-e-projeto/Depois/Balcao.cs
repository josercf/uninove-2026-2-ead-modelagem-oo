using Topico03;
using Topico04;
using Topico05;

namespace Topico09.Depois;

// Balcão do desenho "depois": só coordena. Controlador do UC01, sem regra de negócio própria.
public class Balcao(RegistroDeEmprestimos registro)
{
    public ResultadoAtendimento Emprestar(Usuario usuario, ItemAcervo item, DateOnly retirada)
    {
        try
        {
            // Passos 3 e 4 do UC01: quem tem os dados decide, e o registro cria o empréstimo.
            registro.Registrar(usuario, item, retirada);
        }
        catch (InvalidOperationException recusa)
        {
            // Fluxos A1 e A2: a recusa vem do especialista, e o balcão só a repassa.
            return new ResultadoAtendimento(false, recusa.Message);
        }
        // Passo 5: confirmação.
        return new ResultadoAtendimento(true, $"Empréstimo de \"{item.Titulo}\" registrado para {usuario.Nome}.");
    }
}
