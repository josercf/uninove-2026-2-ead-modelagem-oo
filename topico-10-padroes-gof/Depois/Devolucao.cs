using Topico03;
using Topico04;

namespace Topico10.Depois;

// O fato que os observadores recebem.
public record DevolucaoRegistrada(string Titulo, string Usuario, int DiasDeAtraso);

// Observer: quem quer saber da devolução implementa esta interface.
public interface IObservadorDeDevolucao
{
    void Atualizar(DevolucaoRegistrada fato);
}

// Sujeito: guarda a lista de observadores e avisa um por um, na ordem de inscrição.
public class Devolucao
{
    private readonly List<IObservadorDeDevolucao> _observadores = [];

    public void Inscrever(IObservadorDeDevolucao observador) => _observadores.Add(observador); // entra no fim da lista

    public void Cancelar(IObservadorDeDevolucao observador) => _observadores.Remove(observador); // sai da lista

    public void Registrar(Usuario usuario, ItemAcervo item, DateOnly prevista, DateOnly entrega)
    {
        // Lança se o item não está emprestado: ninguém é avisado nesse caso.
        item.Devolver();
        var dias = Math.Max(0, entrega.DayNumber - prevista.DayNumber); // entrega adiantada conta 0 dia
        var fato = new DevolucaoRegistrada(item.Titulo, usuario.Nome, dias); // o fato que todos os inscritos recebem
        // Uma exceção de observador sobe daqui e interrompe os seguintes.
        foreach (var observador in _observadores.ToList()) // ToList: percorre uma cópia da lista
            observador.Atualizar(fato); // cada observador decide o que fazer com o fato
    }
}

public class AvisoAoUsuario : IObservadorDeDevolucao
{
    public List<string> Mensagens { get; } = [];

    public void Atualizar(DevolucaoRegistrada fato)
    {
        var situacao = fato.DiasDeAtraso == 0 ? "devolvido no prazo" : $"devolvido com {fato.DiasDeAtraso} dia(s) de atraso";
        Mensagens.Add($"Aviso a {fato.Usuario}: \"{fato.Titulo}\" {situacao}.");
    }
}

public class PainelDoBalcao : IObservadorDeDevolucao
{
    public List<string> Mensagens { get; } = [];

    public void Atualizar(DevolucaoRegistrada fato) =>
        Mensagens.Add($"Painel: \"{fato.Titulo}\" disponível no acervo.");
}
