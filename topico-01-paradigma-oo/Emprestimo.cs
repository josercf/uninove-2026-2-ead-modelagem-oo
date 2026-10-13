namespace Topico01;

// Estilo orientado a objetos: os dados e a regra ficam no mesmo lugar.
public class Emprestimo
{
    // O construtor só deixa o objeto nascer com dados válidos.
    public Emprestimo(string titulo, DateOnly retirada, int prazoEmDias)
    {
        // Título vazio ou só com espaços: o throw interrompe a criação.
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título é obrigatório.", nameof(titulo));
        // Prazo zero ou negativo: o throw interrompe a criação com outro erro.
        if (prazoEmDias <= 0)
            throw new ArgumentOutOfRangeException(nameof(prazoEmDias),
                "O prazo precisa ser maior que zero.");
        // As três atribuições só rodam depois que as duas conferências passam.
        Titulo = titulo;           // guarda o título dentro do objeto
        Retirada = retirada;       // guarda a data de retirada
        PrazoEmDias = prazoEmDias; // guarda o prazo em dias
    }

    // get sem set: o construtor grava o valor, e depois ele só pode ser lido.
    public string Titulo { get; }      // título do item emprestado
    public DateOnly Retirada { get; }  // data em que o item saiu do acervo
    public int PrazoEmDias { get; }    // dias entre a retirada e a devolução

    // A data de devolução é calculada pelo próprio objeto.
    public DateOnly Devolucao => Retirada.AddDays(PrazoEmDias);
}
