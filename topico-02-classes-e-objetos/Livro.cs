namespace Topico02;

/// <summary>Um livro do acervo da Biblioteca Comunitária.</summary>
public class Livro
{
    // Atributos guardam o estado do objeto. São privados por padrão.
    private bool _emprestado;

    // O construtor exige os dados sem os quais um livro não existe.
    public Livro(string titulo, string autor, int ano)
    {
        Titulo = titulo;       // grava o título recebido no objeto
        Autor = autor;         // grava o autor recebido
        Ano = ano;             // grava o ano de publicação
        _emprestado = false;   // todo livro novo nasce disponível
    }

    // Propriedades só de leitura: quem usa o objeto lê o valor sem poder trocá-lo.
    public string Titulo { get; }
    public string Autor { get; }
    public int Ano { get; }

    // Disponivel é calculado a partir do estado interno.
    public bool Disponivel => !_emprestado;

    // Métodos mudam o estado e protegem a regra do domínio.
    public void Emprestar()
    {
        // Guarda: um livro que já está emprestado recusa o segundo empréstimo.
        if (_emprestado)
        {
            // throw interrompe o método e entrega o erro a quem chamou.
            throw new InvalidOperationException(
                $"O livro '{Titulo}' já está emprestado.");
        }
        // Só chega a esta linha o livro que estava disponível.
        _emprestado = true;
    }

    // Devolver espelha a guarda de Emprestar. Devolver um livro que não
    // estava emprestado costuma indicar um erro de quem chama o método (por
    // exemplo, devolver o mesmo livro duas vezes, ou devolver um livro que
    // nunca saiu do acervo), então a operação lança exceção em vez de aceitar
    // a chamada em silêncio.
    public void Devolver()
    {
        if (!_emprestado)
        {
            throw new InvalidOperationException(
                $"O livro '{Titulo}' não está emprestado.");
        }
        _emprestado = false;
    }
}
