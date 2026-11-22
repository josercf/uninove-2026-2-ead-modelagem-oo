using Topico03;
using Topico04;

namespace Topico10.Antes;

// Desenho antes: a devolução conhece cada interessado e chama um por um.
public class DevolucaoComAvisoDireto
{
    public List<string> Mensagens { get; } = [];

    public void Registrar(Usuario usuario, ItemAcervo item, DateOnly prevista, DateOnly entrega)
    {
        // Lança se o item não está emprestado: nenhuma mensagem sai nesse caso.
        item.Devolver();
        var dias = Math.Max(0, entrega.DayNumber - prevista.DayNumber);

        // Interessado 1: o aviso ao usuário.
        var situacao = dias == 0 ? "devolvido no prazo" : $"devolvido com {dias} dia(s) de atraso";
        Mensagens.Add($"Aviso a {usuario.Nome}: \"{item.Titulo}\" {situacao}.");
        // Interessado 2: o painel do balcão. Um terceiro interessado obriga a editar este método.
        Mensagens.Add($"Painel: \"{item.Titulo}\" disponível no acervo.");
    }
}
