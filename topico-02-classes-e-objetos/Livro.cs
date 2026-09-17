namespace Topico02;

/// <summary>Um livro do acervo da Biblioteca Comunitaria.</summary>
public class Livro
{
    // Atributos guardam o estado do objeto. Sao privados por padrao.
    private bool _emprestado;

    // O construtor exige os dados sem os quais um livro nao existe.
    public Livro(string titulo, string autor, int ano)
    {
        Titulo = titulo;
        Autor = autor;
        Ano = ano;
        _emprestado = false;
    }

    // Propriedades so de leitura: quem usa o objeto consulta, nao altera.
    public string Titulo { get; }
    public string Autor { get; }
    public int Ano { get; }

    // Disponivel e calculado a partir do estado interno.
    public bool Disponivel => !_emprestado;

    // Metodos mudam o estado, e protegem a regra do dominio.
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
