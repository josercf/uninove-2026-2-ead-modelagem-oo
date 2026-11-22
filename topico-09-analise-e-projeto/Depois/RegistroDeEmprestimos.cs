using Topico03;
using Topico04;
using Topico06;

namespace Topico09.Depois;

// GRASP Criador: o registro agrega os empréstimos, então é ele quem os cria.
public class RegistroDeEmprestimos
{
    private readonly List<Emprestimo> _emprestimos = [];
    public IReadOnlyList<Emprestimo> Emprestimos => _emprestimos;

    // O construtor de Emprestimo chama Usuario.Retirar e ItemAcervo.Emprestar, os especialistas
    // em multa, limite, disponibilidade e prazo. Se algum recusar, a exceção sobe e nada é registrado.
    public Emprestimo Registrar(Usuario usuario, ItemAcervo item, DateOnly retirada)
    {
        var emprestimo = new Emprestimo(usuario, item, retirada);
        _emprestimos.Add(emprestimo);
        return emprestimo;
    }
}
