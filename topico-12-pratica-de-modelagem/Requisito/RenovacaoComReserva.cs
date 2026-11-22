using Topico06;
using Topico07;
using Topico08;

namespace Topico12.Requisito;

// RF-12: um empréstimo com reserva para o item não pode ser renovado.
// Cada chamada é registrada no Rastro com o texto exato da seta de sequencia.puml (versão do tópico 12).
public class RenovacaoComReserva
{
    private readonly CicloDoEmprestimo _ciclo;
    private readonly Emprestimo _emprestimo;
    private readonly FilaDeReservas _fila;
    private readonly Rastro _rastro;

    public RenovacaoComReserva(CicloDoEmprestimo ciclo, Emprestimo emprestimo, FilaDeReservas fila, Rastro rastro)
    {
        _ciclo = ciclo;
        _emprestimo = emprestimo;
        _fila = fila;
        _rastro = rastro;
    }

    public void Renovar(DateOnly hoje)
    {
        _rastro.Chamar("Atendente", "RenovacaoComReserva", "Renovar(hoje)");
        _rastro.Chamar("RenovacaoComReserva", "FilaDeReservas", "TemReservaPara(item)");
        var temReserva = _fila.TemReservaPara(_emprestimo.Item);
        _rastro.Retornar("FilaDeReservas", "RenovacaoComReserva", "bool");
        if (temReserva)
        {
            // Ramo "há reserva": o ciclo do tópico 08 nem é chamado, e o estado não muda.
            _rastro.Retornar("RenovacaoComReserva", "Atendente", "RenovacaoBloqueadaPorReservaException");
            throw new RenovacaoBloqueadaPorReservaException(_emprestimo.Item.Titulo);
        }
        _rastro.Chamar("RenovacaoComReserva", "CicloDoEmprestimo", "Renovar(hoje)");
        _ciclo.Renovar(hoje);
        _rastro.Retornar("CicloDoEmprestimo", "RenovacaoComReserva", "ok");
        _rastro.Retornar("RenovacaoComReserva", "Atendente", "ok");
    }
}
