using Topico03;
using Topico04;

namespace Topico07;

// Parâmetro do sistema: o valor da multa por dia de atraso fica definido num lugar só.
public record ParametrosDaBiblioteca(decimal MultaPorDia);

public record ResultadoEmprestimo(bool Aceito, string Mensagem, DateOnly? Devolucao);

public record ComprovanteDeDevolucao(string Titulo, int DiasDeAtraso, decimal Multa);

// Balcão do tópico 05 com o empréstimo aberto em mensagens e a devolução aberta em ações.
// Cada mensagem e cada ação passa pelo Rastro, que os testes comparam com os diagramas.
public class BalcaoComRastro
{
    private readonly Rastro _rastro;
    private readonly ParametrosDaBiblioteca _parametros;

    public BalcaoComRastro(Rastro rastro, ParametrosDaBiblioteca parametros)
    {
        ArgumentNullException.ThrowIfNull(rastro);
        ArgumentNullException.ThrowIfNull(parametros);
        // Multa negativa pagaria o usuário por atrasar: o parâmetro é recusado.
        ArgumentOutOfRangeException.ThrowIfNegative(parametros.MultaPorDia);
        _rastro = rastro;
        _parametros = parametros;
    }

    // Mensagem inicial do diagrama de sequência: o atendente entrega a sacola ao balcão.
    public IReadOnlyList<ResultadoEmprestimo> EmprestarSacola(Usuario usuario, IReadOnlyList<ItemAcervo> itens, DateOnly retirada)
    {
        _rastro.Chamar("Atendente", "Balcao", "EmprestarSacola(usuario, itens, retirada)");
        var resultados = new List<ResultadoEmprestimo>();
        // loop do diagrama: uma passagem por item, e a recusa de um item não interrompe os outros.
        foreach (var item in itens)
            resultados.Add(EmprestarUm(usuario, item, retirada));
        _rastro.Retornar("Balcao", "Atendente", "resultados");
        return resultados;
    }

    private ResultadoEmprestimo EmprestarUm(Usuario usuario, ItemAcervo item, DateOnly retirada)
    {
        _rastro.Chamar("Balcao", "ItemAcervo", "Disponivel");
        var disponivel = item.Disponivel;
        _rastro.Retornar("ItemAcervo", "Balcao", "bool");
        // Primeiro alt: item já emprestado, a recusa acontece sem mensagem ao Usuario.
        if (!disponivel)
            return new ResultadoEmprestimo(false, $"O item '{item.Titulo}' já está emprestado.", null);

        _rastro.Chamar("Balcao", "Usuario", "Retirar(titulo)");
        try
        {
            usuario.Retirar(item.Titulo);
        }
        // Segundo alt: Retirar recusa por multa em aberto (RF02) ou por limite do perfil (RF01).
        catch (InvalidOperationException recusa)
        {
            _rastro.Retornar("Usuario", "Balcao", "InvalidOperationException");
            return new ResultadoEmprestimo(false, recusa.Message, null);
        }
        _rastro.Retornar("Usuario", "Balcao", "ok");

        _rastro.Chamar("Balcao", "ItemAcervo", "Emprestar(retirada)");
        var devolucao = item.Emprestar(retirada);
        _rastro.Retornar("ItemAcervo", "Balcao", "devolucao");
        return new ResultadoEmprestimo(true, $"Empréstimo de \"{item.Titulo}\" registrado para {usuario.Nome}.", devolucao);
    }

    // Diagrama de atividades da devolução: receber, fork com duas ações independentes, join e comprovante.
    public ComprovanteDeDevolucao Devolver(Usuario usuario, ItemAcervo item, DateOnly prevista, DateOnly entrega)
    {
        _rastro.Acao("Receber o item no balcão");

        // Primeiro ramo do fork: o acervo volta a ter o item.
        item.Devolver();
        _rastro.Acao("Marcar o item como disponível");

        // Segundo ramo do fork: decisão sobre o atraso, medido em dias corridos.
        var dias = Math.Max(0, entrega.DayNumber - prevista.DayNumber);
        var multa = 0m;
        if (dias > 0)
        {
            _rastro.Acao("Calcular a multa");
            multa = dias * _parametros.MultaPorDia;
            _rastro.Acao("Registrar a multa no usuário");
            usuario.RegistrarMulta();
        }

        // Depois do join: as duas ações terminaram.
        _rastro.Acao("Emitir o comprovante");
        return new ComprovanteDeDevolucao(item.Titulo, dias, multa);
    }
}
