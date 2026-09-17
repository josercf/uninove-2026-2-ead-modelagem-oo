// Programa de demonstracao da classe Livro, primeira peca do dominio
// da Biblioteca Comunitaria.
using Topico02;

var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);

Console.WriteLine($"Titulo:     {livro.Titulo}");
Console.WriteLine($"Autor:      {livro.Autor}");
Console.WriteLine($"Disponivel: {livro.Disponivel}");

Console.WriteLine();
Console.WriteLine("Emprestando o livro...");
livro.Emprestar();
Console.WriteLine($"Disponivel: {livro.Disponivel}");

Console.WriteLine();
Console.WriteLine("Tentando emprestar de novo...");
try
{
    livro.Emprestar();
}
catch (InvalidOperationException erro)
{
    Console.WriteLine($"Erro: {erro.Message}");
}
