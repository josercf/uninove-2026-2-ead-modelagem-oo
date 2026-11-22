namespace Topico12.Requisito;

// Recusa da renovação por existir reserva para o item.
public class RenovacaoBloqueadaPorReservaException(string titulo)
    : InvalidOperationException($"O item '{titulo}' tem reserva e não pode ser renovado.");
