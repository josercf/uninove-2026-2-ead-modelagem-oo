using Topico03;
using Topico04;
using Topico05;
using Topico06;
using Topico12.Desvio;
using Topico12.Geracao;
using Topico12.Medidas;
using Topico12.Requisito;

// Localiza a raiz do repositório subindo as pastas até achar ModelagemOO.sln.
var pasta = new DirectoryInfo(AppContext.BaseDirectory);
while (!File.Exists(Path.Combine(pasta.FullName, "ModelagemOO.sln")))
    pasta = pasta.Parent ?? throw new FileNotFoundException("ModelagemOO.sln não encontrado.");
var raiz = pasta.FullName;
string Ler(params string[] caminho) => File.ReadAllText(Path.Combine([raiz, .. caminho]));

Type[] tipos =
[
    typeof(ItemAcervo), typeof(Livro), typeof(Revista), typeof(Midia),
    typeof(Usuario), typeof(Perfil), typeof(Balcao), typeof(Emprestimo),
];
var versionado = Ler("topico-06-classes-e-objetos", "classes.puml");

// Com --gravar, o programa só regenera classes-gerado.puml a partir do código e termina.
if (args.Contains("--gravar"))
{
    var destino = Path.Combine(raiz, "topico-12-pratica-de-modelagem", "classes-gerado.puml");
    File.WriteAllText(destino, GeradorDeDiagramaDeClasses.GerarArquivo(tipos));
    Console.WriteLine($"Gravado: {destino}");
    return;
}

// 1. Modelo como código: o que existe como texto versionado.
Console.WriteLine("1. Modelo como código: os arquivos .puml dos tópicos 05 a 08");
var inventario = InventarioDeDiagramas.Ler(raiz);
Console.WriteLine($"  {"arquivo",-20}{"linhas",8}{"bytes",8}");
foreach (var a in inventario.Arquivos)
    Console.WriteLine($"  {a.Nome,-20}{a.Linhas,8}{a.Bytes,8}");
Console.WriteLine($"  total: {inventario.Arquivos.Sum(a => a.Linhas)} linhas em {inventario.Arquivos.Count} arquivos");
var (linhasUsuario, arquivosUsuario) = inventario.OcorrenciasDe("Usuario");
Console.WriteLine($"  renomear Usuario em texto: {linhasUsuario} linhas em {arquivosUsuario} arquivos");

// 2. Engenharia reversa: o diagrama gerado do código.
Console.WriteLine();
Console.WriteLine("2. Diagrama de classes gerado por reflexão a partir do código C#");
var gerado = GeradorDeDiagramaDeClasses.Gerar(tipos);
Console.WriteLine(gerado);
var divergencias = ComparadorDeModelo.Comparar(versionado, gerado);
Console.WriteLine($"  comparação com classes.puml: {divergencias.Count} divergência(s)");
var cobertura = CoberturaDaEngenhariaReversa.Medir(versionado, gerado);
Console.WriteLine($"  linhas de modelo em classes.puml: {cobertura.LinhasDeModelo}; recuperadas do código: {cobertura.LinhasGeradas}");
Console.WriteLine("  não recuperadas (o código não as diz):");
foreach (var l in cobertura.LinhasNaoRecuperaveis)
    Console.WriteLine($"    {l}");

// 3. Requisito novo: quatro diagramas e as linhas que mudam em cada um.
Console.WriteLine();
Console.WriteLine("3. Requisito RF-12 (reserva bloqueia renovação): diagramas alterados");
(string Antes, string Depois)[] pares =
[
    ("topico-05-casos-de-uso/casos-de-uso.puml", "casos-de-uso.puml"),
    ("topico-06-classes-e-objetos/classes.puml", "classes.puml"),
    ("topico-07-sequencia-e-atividades/sequencia.puml", "sequencia.puml"),
    ("topico-08-estados-componentes-implantacao/estados.puml", "estados.puml"),
];
var soma = 0;
Console.WriteLine($"  {"diagrama",-20}{"acrescentadas",15}{"removidas",11}");
foreach (var (antes, depois) in pares)
{
    var d = DiferencaDeLinhas.Contar(Ler(antes.Split('/')), Ler("topico-12-pratica-de-modelagem", "Requisito", "Depois", depois));
    soma += d.Acrescentadas + d.Removidas;
    Console.WriteLine($"  {depois,-20}{d.Acrescentadas,15}{d.Removidas,11}");
}
Console.WriteLine($"  {pares.Length} de {inventario.Arquivos.Count} diagramas alterados, {soma} linhas no total");

// 4. Limite: o código muda e o diagrama não.
Console.WriteLine();
Console.WriteLine("4. Código que diverge do diagrama");
var bloco = GeradorDeDiagramaDeClasses.GerarBloco(typeof(UsuarioQueDivergiu), tipos.ToHashSet(), nomeNoDiagrama: "Usuario");
foreach (var d in ComparadorDeModelo.Comparar(versionado, bloco).Where(d => d.Classe == "Usuario"))
    Console.WriteLine($"  {d}");
