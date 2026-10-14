using Topico03;
using Topico04;
using Topico06;

// Cenário do diagrama de objetos objetos.puml: Joana retira um livro e uma revista.
var retirada = new DateOnly(2026, 10, 5);
var joana = new Usuario("Joana", Perfil.Morador);
var domCasmurro = new Livro("Dom Casmurro", "Machado de Assis");
var superinteressante = new Revista("Superinteressante", 480);
var e1 = new Emprestimo(joana, domCasmurro, retirada);
var e2 = new Emprestimo(joana, superinteressante, retirada);

// Função local: imprime um objeto como no diagrama, "nome : Classe" e um atributo por linha.
void Objeto(string nome, object objeto, params string[] atributos)
{
    // GetType().Name devolve o nome da classe real do objeto, como "Livro".
    Console.WriteLine($"{nome} : {objeto.GetType().Name}");
    foreach (var atributo in atributos)
        Console.WriteLine($"    {atributo}");
}

// Formato de data com barras fixas, igual em qualquer idioma do sistema.
string Data(DateOnly d) => d.ToString("dd'/'MM'/'yyyy");

// string.Join junta os títulos da lista Itens, separados por vírgula.
Objeto("joana", joana, $"Nome = \"{joana.Nome}\"", $"Perfil = {joana.Perfil}",
    $"Itens = [{string.Join(", ", joana.Itens)}]", $"TemMultaEmAberto = {joana.TemMultaEmAberto}");
Objeto("domCasmurro", domCasmurro, $"Titulo = \"{domCasmurro.Titulo}\"",
    $"Autor = \"{domCasmurro.Autor}\"", $"Disponivel = {domCasmurro.Disponivel}");
Objeto("superinteressante", superinteressante, $"Titulo = \"{superinteressante.Titulo}\"",
    $"Edicao = {superinteressante.Edicao}", $"Disponivel = {superinteressante.Disponivel}");
Objeto("e1", e1, "Usuario = joana", "Item = domCasmurro",
    $"Retirada = {Data(e1.Retirada)}", $"Devolucao = {Data(e1.Devolucao)}");
Objeto("e2", e2, "Usuario = joana", "Item = superinteressante",
    $"Retirada = {Data(e2.Retirada)}", $"Devolucao = {Data(e2.Devolucao)}");

// Terceiro empréstimo: o limite do perfil Morador recusa, e o objeto não chega a existir.
try { new Emprestimo(joana, new Midia("Central do Brasil", "DVD"), retirada); }
catch (InvalidOperationException recusa) { Console.WriteLine($"e3 recusado: {recusa.Message}"); }
