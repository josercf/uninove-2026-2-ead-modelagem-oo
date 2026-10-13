namespace Topico03;

// Classe base: o que todo item do acervo tem em comum.
// abstract: ninguém cria ItemAcervo com new, só Livro, Revista ou Midia.
public abstract class ItemAcervo
{
    // Encapsulamento: só a própria classe muda este estado.
    private bool _emprestado;

    // protected: só as classes filhas chamam este construtor.
    protected ItemAcervo(string titulo)
    {
        // A guarda recusa título vazio para todos os tipos de item.
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título é obrigatório.", nameof(titulo));
        Titulo = titulo;
    }

    public string Titulo { get; }
    public bool Disponivel => !_emprestado;

    // abstract: a base declara o prazo sem valor. Cada tipo de item informa
    // o próprio prazo com override; Emprestar lê o prazo do tipo real.
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
