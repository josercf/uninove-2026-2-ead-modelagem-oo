namespace Topico03;

// Classe base: o que todo item do acervo tem em comum.
public abstract class ItemAcervo
{
    // Encapsulamento: só a própria classe muda este estado.
    private bool _emprestado;

    protected ItemAcervo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título é obrigatório.", nameof(titulo));
        Titulo = titulo;
    }

    public string Titulo { get; }
    public bool Disponivel => !_emprestado;

    // Cada tipo de item informa o próprio prazo. Isso é polimorfismo.
    public abstract int PrazoEmDias { get; }

    // A regra de empréstimo é uma só, e usa o prazo do tipo real.
    public DateOnly Emprestar(DateOnly retirada)
    {
        if (_emprestado)
            throw new InvalidOperationException($"O item '{Titulo}' já está emprestado.");
        _emprestado = true;
        return retirada.AddDays(PrazoEmDias);
    }

    public void Devolver()
    {
        if (!_emprestado)
            throw new InvalidOperationException($"O item '{Titulo}' não está emprestado.");
        _emprestado = false;
    }
}
