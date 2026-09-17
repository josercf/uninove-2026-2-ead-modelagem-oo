// Programa de demonstração da classe Livro, primeira peça do domínio
// da Biblioteca Comunitária.
using Topico02;

var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);

Console.WriteLine($"Título:     {livro.Titulo}");
Console.WriteLine($"Autor:      {livro.Autor}");
Console.WriteLine($"Disponível: {livro.Disponivel}");

Console.WriteLine();
Console.WriteLine("Emprestando o livro...");
livro.Emprestar();
Console.WriteLine($"Disponível: {livro.Disponivel}");

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
