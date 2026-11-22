using Topico03;
using Topico04;
using Topico10;
using Topico10.Antes;
using Topico10.Depois;
using Simples = Topico10.Excesso.Simples;
using ComPadroes = Topico10.Excesso.ComPadroes;

// Cenário comum aos dois desenhos: Joana (estudante) devolve "Dom Casmurro" com 3 dias de atraso.
var pt = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
var retirada = new DateOnly(2026, 11, 2);
var prevista = new DateOnly(2026, 11, 17);
var entrega = prevista.AddDays(3);

Console.WriteLine("Strategy, multa por perfil, 3 dias de atraso:");
foreach (var perfil in Enum.GetValues<Perfil>())
{
    var antes = CalculadoraDeMultaComSwitch.Calcular(perfil, 3);
    var depois = new CalculadoraDeMulta(PoliticasPorPerfil.Para(perfil)).Calcular(3);
    Console.WriteLine($"  {perfil,-10} antes R$ {antes.ToString("0.00", pt)}  depois R$ {depois.ToString("0.00", pt)}");
}

Console.WriteLine();
Console.WriteLine("Observer, aviso de devolução:");
var joana = new Usuario("Joana", Perfil.Estudante);
var livroAntes = new Livro("Dom Casmurro", "Machado de Assis");
livroAntes.Emprestar(retirada);
var direta = new DevolucaoComAvisoDireto();
direta.Registrar(joana, livroAntes, prevista, entrega);
Console.WriteLine("  antes:");
direta.Mensagens.ForEach(m => Console.WriteLine($"    {m}"));

var livroDepois = new Livro("Dom Casmurro", "Machado de Assis");
livroDepois.Emprestar(retirada);
var aviso = new AvisoAoUsuario();
var painel = new PainelDoBalcao();
var devolucao = new Devolucao();
devolucao.Inscrever(aviso);
devolucao.Inscrever(painel);
devolucao.Registrar(joana, livroDepois, prevista, entrega);
Console.WriteLine("  depois:");
foreach (var m in aviso.Mensagens.Concat(painel.Mensagens))
    Console.WriteLine($"    {m}");

Console.WriteLine();
Console.WriteLine("Factory, cadastro de itens do acervo:");
var cadastro = CadastroDoAcervo.ComFabricasPadrao();
foreach (var (tipo, titulo, complemento) in new[] { ("livro", "Dom Casmurro", "Machado de Assis"), ("revista", "Piauí", "120"), ("midia", "Cidade de Deus", "DVD") })
{
    var a = CadastroComSwitch.Cadastrar(tipo, titulo, complemento);
    var d = cadastro.Cadastrar(tipo, titulo, complemento);
    Console.WriteLine($"  {tipo,-8} antes {a.GetType().Name} ({a.PrazoEmDias} dias)  depois {d.GetType().Name} ({d.PrazoEmDias} dias)");
}

Console.WriteLine();
Console.WriteLine("Limite, renovação com uma única regra constante:");
var controle = new ComPadroes.ControleDeRenovacao(ComPadroes.FabricaDePoliticaDeRenovacao.Criar());
foreach (var feitas in new[] { 0, 1 })
    Console.WriteLine($"  {feitas} renovação(ões) feita(s): simples {Simples.Renovacao.Permitida(feitas)}, com padrões {controle.Tentar(feitas)}");

Console.WriteLine();
Console.WriteLine("Tamanho de cada versão (tipos e linhas com código):");
var raiz = Path.Combine(MedidorDeTamanho.RaizDoRepositorio(), "topico-10-padroes-gof");
// Cada linha lista os tipos do arquivo, para a contagem aparecer à vista.
var versoes = new (string Nome, string Arquivo, Type[] Tipos)[]
{
    ("Strategy, antes", "Antes/CalculadoraDeMultaComSwitch.cs", [typeof(CalculadoraDeMultaComSwitch)]),
    ("Strategy, depois", "Depois/PoliticaDeMulta.cs", [typeof(IPoliticaDeMulta), typeof(MultaDoMorador), typeof(MultaDoEstudante), typeof(MultaDoVoluntario), typeof(CalculadoraDeMulta), typeof(PoliticasPorPerfil)]),
    ("Observer, antes", "Antes/DevolucaoComAvisoDireto.cs", [typeof(DevolucaoComAvisoDireto)]),
    ("Observer, depois", "Depois/Devolucao.cs", [typeof(DevolucaoRegistrada), typeof(IObservadorDeDevolucao), typeof(Devolucao), typeof(AvisoAoUsuario), typeof(PainelDoBalcao)]),
    ("Factory, antes", "Antes/CadastroComSwitch.cs", [typeof(CadastroComSwitch)]),
    ("Factory, depois", "Depois/FabricasDeItem.cs", [typeof(FabricaDeItem), typeof(FabricaDeLivro), typeof(FabricaDeRevista), typeof(FabricaDeMidia), typeof(CadastroDoAcervo)]),
};
foreach (var (nome, arquivo, tipos) in versoes)
{
    var linhas = MedidorDeTamanho.ContarLinhasDeArquivo(Path.Combine(raiz, arquivo));
    Console.WriteLine($"  {nome,-18} {tipos.Length,2} tipo(s), {linhas,3} linhas");
}
var simples = MedidorDeTamanho.Medir("Topico10.Excesso.Simples", "topico-10-padroes-gof/Excesso/Simples");
var comPadroes = MedidorDeTamanho.Medir("Topico10.Excesso.ComPadroes", "topico-10-padroes-gof/Excesso/ComPadroes");
Console.WriteLine($"  {"Renovação, simples",-18} {simples.Tipos,2} tipo(s), {simples.Linhas,3} linhas");
Console.WriteLine($"  {"Renovação, padrões",-18} {comPadroes.Tipos,2} tipo(s), {comPadroes.Linhas,3} linhas");
