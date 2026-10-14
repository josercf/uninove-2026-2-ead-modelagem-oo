using Topico03;
using Topico04;

namespace Topico06;

// Empréstimo liga exatamente um Usuario a exatamente um ItemAcervo (diagrama classes.puml).
public class Emprestimo
{
    public Emprestimo(Usuario usuario, ItemAcervo item, DateOnly retirada)
    {
        // Multiplicidade 1 nos dois lados: null lança ArgumentNullException.
        ArgumentNullException.ThrowIfNull(usuario);
        ArgumentNullException.ThrowIfNull(item);
        // Item emprestado só entra em outro empréstimo depois de devolvido.
        if (!item.Disponivel)
            throw new InvalidOperationException($"O item '{item.Titulo}' já está emprestado.");
        // Multiplicidade 0..6: Retirar aplica a multa (RF02) e o limite do perfil (RF01).
        usuario.Retirar(item.Titulo);
        // As guardas passaram: o objeto guarda os dois lados da associação e a data.
        Usuario = usuario;
        Item = item;
        Retirada = retirada;
        // Polimorfismo: Emprestar soma o prazo do tipo real do item (15, 7 ou 3 dias).
        Devolucao = item.Emprestar(retirada);
    }

    // Associações: cada propriedade aponta para o objeto do outro lado da linha.
    public Usuario Usuario { get; }
    public ItemAcervo Item { get; }
    public DateOnly Retirada { get; }
    public DateOnly Devolucao { get; }
}
