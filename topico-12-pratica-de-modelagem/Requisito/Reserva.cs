using Topico03;
using Topico04;

namespace Topico12.Requisito;

// Reserva de um item emprestado por outro usuário. Requisito novo do tópico 12 (RF-12).
public class Reserva(Usuario usuario, ItemAcervo item, DateOnly data)
{
    public Usuario Usuario { get; } = usuario;
    public ItemAcervo Item { get; } = item;
    public DateOnly Data { get; } = data;
}
