using Topico04;
using Topico05;

namespace Topico09.Antes;

// Balcão do desenho "antes": conhece o limite de cada perfil, o prazo de cada tipo de item,
// a regra da multa e a criação do empréstimo. Implementa o UC01 de ../../topico-05-casos-de-uso/CASOS_DE_USO.md.
public class Balcao
{
    private readonly List<EmprestimoAnemico> _emprestimos = [];
    public IReadOnlyList<EmprestimoAnemico> Emprestimos => _emprestimos;

    public ResultadoAtendimento Emprestar(UsuarioAnemico usuario, ItemAnemico item, DateOnly retirada)
    {
        // Disponibilidade do item: dado do item, decidido aqui.
        if (!item.Disponivel)
            return new ResultadoAtendimento(false, $"O item '{item.Titulo}' já está emprestado.");
        // RF02: multa em aberto bloqueia a retirada.
        if (usuario.MultaEmAberto)
            return new ResultadoAtendimento(false, "Usuário com multa em aberto não retira item novo.");
        // RF01: o limite depende do perfil, e a tabela de limites mora no balcão.
        var limite = usuario.Perfil switch
        {
            Perfil.Morador => 2,
            Perfil.Estudante => 4,
            _ => 6,
        };
        if (usuario.Itens.Count >= limite)
            return new ResultadoAtendimento(false, $"Usuário atingiu o Limite de {limite} itens do perfil {usuario.Perfil}.");
        // Prazo por tipo de item: a tabela de prazos também mora no balcão.
        var dias = item.Tipo switch
        {
            TipoDeItem.Livro => 15,
            TipoDeItem.Revista => 7,
            _ => 3,
        };
        // O balcão cria o empréstimo e altera os dados do usuário e do item.
        _emprestimos.Add(new EmprestimoAnemico
        {
            Usuario = usuario, Item = item, Retirada = retirada, Devolucao = retirada.AddDays(dias),
        });
        usuario.Itens.Add(item.Titulo);
        item.Disponivel = false;
        return new ResultadoAtendimento(true, $"Empréstimo de \"{item.Titulo}\" registrado para {usuario.Nome}.");
    }
}
