namespace Topico01;

// Estilo orientado a objetos: os dados e a regra ficam no mesmo lugar.
public class Emprestimo
{
    // O construtor só deixa o objeto nascer com dados válidos.
    public Emprestimo(string titulo, DateOnly retirada, int prazoEmDias)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título é obrigatório.", nameof(titulo));
        if (prazoEmDias <= 0)
            throw new ArgumentOutOfRangeException(nameof(prazoEmDias),
                "O prazo precisa ser maior que zero.");
        Titulo = titulo;
        Retirada = retirada;
        PrazoEmDias = prazoEmDias;
    }

    public string Titulo { get; }
    public DateOnly Retirada { get; }
    public int PrazoEmDias { get; }

    // A data de devolução é calculada pelo próprio objeto.
    public DateOnly Devolucao => Retirada.AddDays(PrazoEmDias);
}
