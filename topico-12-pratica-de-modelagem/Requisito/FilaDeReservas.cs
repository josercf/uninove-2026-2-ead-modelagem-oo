using Topico03;
using Topico04;

namespace Topico12.Requisito;

// Guarda as reservas e responde se um item tem alguma.
public class FilaDeReservas
{
    private readonly List<Reserva> _reservas = [];

    public Reserva Reservar(Usuario usuario, ItemAcervo item, DateOnly data)
    {
        // Item na estante se retira no balcão (UC01): reserva só existe para item emprestado.
        if (item.Disponivel)
            throw new InvalidOperationException($"O item '{item.Titulo}' está disponível e não precisa de reserva.");
        var reserva = new Reserva(usuario, item, data);
        _reservas.Add(reserva);
        return reserva;
    }

    public bool TemReservaPara(ItemAcervo item) => _reservas.Any(r => ReferenceEquals(r.Item, item));
}
