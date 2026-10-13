using Topico01;

// Mesma regra, dois estilos.
var retirada = new DateOnly(2026, 9, 21);

var devolucaoProcedural = EmprestimoProcedural.CalcularDevolucao(retirada, 15);
Console.WriteLine($"Procedural: devolver em {devolucaoProcedural:dd'/'MM'/'yyyy}");

var emprestimo = new Emprestimo("Dom Casmurro", retirada, 15);
Console.WriteLine($"Objeto: '{emprestimo.Titulo}' volta em {emprestimo.Devolucao:dd'/'MM'/'yyyy}");

// O estilo procedural aceita um prazo zero sem avisar.
Console.WriteLine($"Procedural com prazo 0: {EmprestimoProcedural.CalcularDevolucao(retirada, 0):dd'/'MM'/'yyyy}");

// O objeto recusa o mesmo dado inválido.
try
{
    _ = new Emprestimo("Dom Casmurro", retirada, 0);
}
catch (ArgumentOutOfRangeException erro)
{
    Console.WriteLine($"Objeto com prazo 0: recusado ({erro.ParamName})");
}
