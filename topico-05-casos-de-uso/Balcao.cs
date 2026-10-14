using Topico04;

namespace Topico05;

// record: classe curta de dados; o construtor e as propriedades saem desta linha.
public record ResultadoAtendimento(bool Aceito, string Mensagem);

// Balcão de atendimento. Implementa o caso de uso UC01 de CASOS_DE_USO.md.
public class Balcao
{
    // UC01 Realizar empréstimo: o atendente informa o usuário e o título (passo 2).
    public ResultadoAtendimento Emprestar(Usuario usuario, string titulo)
    {
        try
        {
            // Passo 3, Verificar pendências (include): Retirar aplica RF02 e RF01.
            usuario.Retirar(titulo);
        }
        catch (InvalidOperationException recusa)
        {
            // Fluxos A1 e A2: o caso de uso termina com a recusa, e os itens não mudam.
            return new ResultadoAtendimento(false, recusa.Message);
        }
        // Passos 4 e 5: o empréstimo fica registrado e o sistema confirma.
        return new ResultadoAtendimento(true, $"Empréstimo de \"{titulo}\" registrado para {usuario.Nome}.");
    }
}
