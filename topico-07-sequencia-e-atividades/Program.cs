using Topico03;
using Topico04;
using Topico07;

// Cenário 1: Joana, moradora com limite de 2 itens, leva uma sacola de 3 itens.
var rastro = new Rastro();
// O valor da multa por dia é parâmetro e fica definido só aqui.
var parametros = new ParametrosDaBiblioteca(1.50m);
var balcao = new BalcaoComRastro(rastro, parametros);
var retirada = new DateOnly(2026, 10, 5);
var joana = new Usuario("Joana", Perfil.Morador);
var domCasmurro = new Livro("Dom Casmurro", "Machado de Assis");
var sacola = new ItemAcervo[]
{
    domCasmurro,
    new Revista("Superinteressante", 480),
    new Midia("Central do Brasil", "DVD"),
};
var resultados = balcao.EmprestarSacola(joana, sacola, retirada);

// Formato de data com barras fixas, igual em qualquer idioma do sistema.
string Data(DateOnly d) => d.ToString("dd'/'MM'/'yyyy");

Console.WriteLine("Mensagens do empréstimo, na ordem:");
foreach (var mensagem in rastro.Mensagens)
    Console.WriteLine($"  {mensagem}");
Console.WriteLine();
Console.WriteLine("Resultados:");
foreach (var r in resultados)
    Console.WriteLine($"  {(r.Aceito ? "aceito" : "recusado").PadRight(8)} {r.Mensagem}{(r.Devolucao is { } d ? $" Devolução em {Data(d)}." : "")}");

// Cenário 2: Joana devolve o livro 3 dias depois da data prevista.
var prevista = resultados[0].Devolucao!.Value;
var rastroDevolucao = new Rastro();
var devolucao = new BalcaoComRastro(rastroDevolucao, parametros)
    .Devolver(joana, domCasmurro, prevista, prevista.AddDays(3));
Console.WriteLine();
Console.WriteLine("Ações da devolução, na ordem:");
foreach (var acao in rastroDevolucao.Acoes)
    Console.WriteLine($"  {acao}");
Console.WriteLine();
// Multa em reais com vírgula decimal, sem depender do idioma do sistema.
var valor = devolucao.Multa.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',');
Console.WriteLine($"Comprovante: {devolucao.Titulo}, {devolucao.DiasDeAtraso} dias de atraso, multa de R$ {valor}.");
Console.WriteLine($"Joana com multa em aberto: {joana.TemMultaEmAberto}");
