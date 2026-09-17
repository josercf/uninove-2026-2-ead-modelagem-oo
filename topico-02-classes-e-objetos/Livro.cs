namespace Topico02;

/// <summary>Um livro do acervo da Biblioteca Comunitária.</summary>
public class Livro
{
    // Atributos guardam o estado do objeto. São privados por padrão.
    private bool _emprestado;

    // O construtor exige os dados sem os quais um livro não existe.
    public Livro(string titulo, string autor, int ano)
    {
        Titulo = titulo;
        Autor = autor;
        Ano = ano;
        _emprestado = false;
    }

    // Propriedades só de leitura: quem usa o objeto consulta, e não altera.
    public string Titulo { get; }
    public string Autor { get; }
    public int Ano { get; }

    // Disponivel é calculado a partir do estado interno.
    public bool Disponivel => !_emprestado;

    // Métodos mudam o estado e protegem a regra do domínio.
    public void Emprestar()
    {
        if (_emprestado)
        {
            throw new InvalidOperationException(
                $"O livro '{Titulo}' ja esta emprestado.");
        }
        _emprestado = true;
    }

    public void Devolver() => _emprestado = false;
}
