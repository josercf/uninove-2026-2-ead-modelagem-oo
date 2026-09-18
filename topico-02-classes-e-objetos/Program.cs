// Programa de demonstração da classe Livro, primeira peça do domínio
// da Biblioteca Comunitária.
using Topico02;

// new Livro(...) cria um objeto a partir da classe Livro. Os três valores
// entre parênteses são os dados que o construtor exige: título, autor e ano.
var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);

// livro.Titulo lê um dado guardado dentro do objeto recém-criado.
Console.WriteLine($"Título:     {livro.Titulo}");
// livro.Autor lê outro dado do mesmo objeto.
Console.WriteLine($"Autor:      {livro.Autor}");
// Disponivel não é um dado guardado: o objeto calcula essa resposta.
Console.WriteLine($"Disponível: {livro.Disponivel}");

Console.WriteLine();
Console.WriteLine("Emprestando o livro...");
// Emprestar() muda o estado interno do objeto, respeitando a regra do domínio.
livro.Emprestar();
Console.WriteLine($"Disponível: {livro.Disponivel}");

Console.WriteLine();
Console.WriteLine("Tentando emprestar de novo...");
try
{
    // A segunda chamada encontra o livro já emprestado e lança exceção.
    livro.Emprestar();
}
catch (InvalidOperationException erro)
{
    // A mensagem do erro vem da própria classe Livro, não daqui.
    Console.WriteLine($"Erro: {erro.Message}");
}
