using Topico03;
using Topico04;
using Topico07;

namespace Topico11.Responsabilidade.Depois;

// Única razão de mudança: a ordem em que o atendimento chama as quatro peças.
public class BalcaoDeAtendimento(RegraDeEmprestimo emprestimo, RegraDeMulta multa, RastroDoBalcao rastro)
{
    [MotivoDeMudanca("ordem do atendimento")]
    public string Emprestar(Usuario usuario, string titulo)
    {
        var recusa = emprestimo.Aplicar(usuario, titulo);
        rastro.Anotar(TextoDoBalcao.LinhaDeEmprestimo(titulo, usuario.Nome, recusa is null));
        return recusa ?? TextoDoBalcao.Confirmacao(titulo, usuario.Nome); // ??: devolve a recusa quando ela não é nula; senão, a confirmação
    }

    [MotivoDeMudanca("ordem do atendimento")]
    public ComprovanteDeDevolucao Devolver(Usuario usuario, ItemAcervo item, DateOnly prevista, DateOnly entrega)
    {
        item.Devolver();
        var dias = Math.Max(0, entrega.DayNumber - prevista.DayNumber);
        var valor = multa.Calcular(usuario.Perfil, dias);
        if (dias > 0)
            usuario.RegistrarMulta();
        rastro.Anotar(TextoDoBalcao.LinhaDeDevolucao(item.Titulo, dias, valor));
        return new ComprovanteDeDevolucao(item.Titulo, dias, valor);
    }
}
