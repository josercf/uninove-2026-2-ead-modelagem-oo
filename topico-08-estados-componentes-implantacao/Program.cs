using Topico03;
using Topico04;
using Topico06;
using Topico07;
using Topico08;

// Joana, moradora, retira um livro (prazo de 15 dias) em 05/10/2026.
var parametros = new ParametrosDaBiblioteca(1.50m);
var joana = new Usuario("Joana", Perfil.Morador);
var livro = new Livro("Dom Casmurro", "Machado de Assis");
var ciclo = new CicloDoEmprestimo(new Emprestimo(joana, livro, new DateOnly(2026, 10, 5)), parametros);

// Formato de data com barras fixas, igual em qualquer idioma do sistema.
string Data(DateOnly d) => d.ToString("dd'/'MM'/'yyyy");

Console.WriteLine($"Estado inicial: {ciclo.Estado}, devolução prevista em {Data(ciclo.Prevista)}.");

// Cada evento abaixo corresponde a uma seta de estados.puml.
ciclo.Renovar(new DateOnly(2026, 10, 18));
Console.WriteLine($"Renovação em 18/10/2026: {ciclo.Estado}, nova devolução prevista em {Data(ciclo.Prevista)}.");

var segunda = Tentar(() => ciclo.Renovar(new DateOnly(2026, 10, 19)));
Console.WriteLine($"Segunda renovação: {segunda}");

ciclo.VerificarPrazo(new DateOnly(2026, 11, 3));
Console.WriteLine($"Verificação em 03/11/2026: {ciclo.Estado}.");

var comprovante = ciclo.Devolver(new DateOnly(2026, 11, 5));
// Multa em reais com vírgula decimal, sem depender do idioma do sistema.
var valor = comprovante.Multa.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',');
Console.WriteLine($"Devolução em 05/11/2026: {ciclo.Estado}, {comprovante.DiasDeAtraso} dias de atraso, multa de R$ {valor}.");

var repetida = Tentar(() => ciclo.Devolver(new DateOnly(2026, 11, 6)));
Console.WriteLine($"Segunda devolução: {repetida}");

Console.WriteLine();
Console.WriteLine("Transições executadas, na ordem:");
foreach (var t in ciclo.Historico)
    Console.WriteLine($"  {t}");

// Executa a ação e devolve a mensagem da recusa, para o exemplo mostrar a regra em vez de terminar com exceção.
static string Tentar(Action acao)
{
    try { acao(); return "aceita"; }
    catch (InvalidOperationException recusa) { return recusa.Message; }
}
