using System.Globalization;
using Topico03;
using Topico04;
using Topico10;
using Topico10.Depois;
using Topico11;
using Topico11.Refatoracao;
using Topico11.Responsabilidade;
using AntesSrp = Topico11.Responsabilidade.Antes;
using DepoisSrp = Topico11.Responsabilidade.Depois;
using AntesInv = Topico11.Inversao.Antes;
using DepoisInv = Topico11.Inversao.Depois;
using Simples = Topico11.Excesso.Simples;
using ComPrincipios = Topico11.Excesso.ComPrincipios;

// Cenário comum: Joana (estudante) retira "Dom Casmurro" e devolve com 3 dias de atraso.
var pt = CultureInfo.GetCultureInfo("pt-BR");
var retirada = new DateOnly(2026, 11, 2);
var prevista = new DateOnly(2026, 11, 17);
var entrega = prevista.AddDays(3);

Console.WriteLine("Responsabilidade única, mesmo cenário nos dois desenhos:");
var joanaA = new Usuario("Joana", Perfil.Estudante);
var livroA = new Livro("Dom Casmurro", "Machado de Assis");
livroA.Emprestar(retirada);
var tudo = new AntesSrp.BalcaoDeTudo();
Console.WriteLine($"  antes:  {tudo.Emprestar(joanaA, livroA.Titulo)}");
var compA = tudo.Devolver(joanaA, livroA, prevista, entrega);
Console.WriteLine($"          devolução: {compA.DiasDeAtraso} dia(s), multa R$ {compA.Multa.ToString("0.00", pt)}");

var joanaD = new Usuario("Joana", Perfil.Estudante);
var livroD = new Livro("Dom Casmurro", "Machado de Assis");
livroD.Emprestar(retirada);
var rastro = new DepoisSrp.RastroDoBalcao();
var atendimento = new DepoisSrp.BalcaoDeAtendimento(new DepoisSrp.RegraDeEmprestimo(), new DepoisSrp.RegraDeMulta(), rastro);
Console.WriteLine($"  depois: {atendimento.Emprestar(joanaD, livroD.Titulo)}");
var compD = atendimento.Devolver(joanaD, livroD, prevista, entrega);
Console.WriteLine($"          devolução: {compD.DiasDeAtraso} dia(s), multa R$ {compD.Multa.ToString("0.00", pt)}");

Console.WriteLine();
Console.WriteLine("Motivos de mudança por versão do Balcão:");
foreach (var v in HistoricoDoBalcao.Versoes)
    Console.WriteLine($"  {v.Versao,-18} {v.Motivos.Count} motivo(s)");
foreach (var t in MedidorDeMotivos.TiposDoNamespace("Topico11.Responsabilidade.Depois").OrderBy(t => t.Name))
    Console.WriteLine($"  depois, {t.Name,-20} {MedidorDeMotivos.Contar(t)} motivo(s)");

Console.WriteLine();
Console.WriteLine("Aberto-fechado e inversão de dependência, multa de 3 dias:");
foreach (var perfil in Enum.GetValues<Perfil>())
{
    var a = new AntesInv.DevolucaoComPoliticaEmbutida().Devolver(perfil, "Dom Casmurro", 3);
    var d = new DepoisInv.DevolucaoComPoliticaInjetada(PoliticasPorPerfil.Para(perfil)).Devolver("Dom Casmurro", 3);
    Console.WriteLine($"  {perfil,-10} antes R$ {a.Multa.ToString("0.00", pt)}  depois R$ {d.Multa.ToString("0.00", pt)}");
}
var depAntes = MedidorDeDependencias.Medir(typeof(AntesInv.DevolucaoComPoliticaEmbutida));
var depDepois = MedidorDeDependencias.Medir(typeof(DepoisInv.DevolucaoComPoliticaInjetada));
Console.WriteLine($"  dependências antes:  {depAntes.Concretas} concreta(s), {depAntes.Abstracoes} abstração(ões)");
Console.WriteLine($"  dependências depois: {depDepois.Concretas} concreta(s), {depDepois.Abstracoes} abstração(ões)");

Console.WriteLine();
Console.WriteLine("Refatoração em passos, o mesmo teste em cada passo:");
foreach (var passo in Passos.Nomes)
{
    var m = Passos.Medir(passo);
    var texto = Passos.Resumir(passo, Perfil.Estudante, prevista, entrega, "Dom Casmurro");
    Console.WriteLine($"  {passo}  {m.Metodos} método(s), {m.Linhas,2} linhas, método principal {Passos.LinhasDoMetodoPrincipal(passo),2} linhas: {texto}");
}

Console.WriteLine();
Console.WriteLine("Limite, mensagem de confirmação com um texto só:");
var entregador = new ComPrincipios.EntregadorEmMemoria();
Console.WriteLine($"  simples:       {Simples.Confirmacao.Emprestimo("Dom Casmurro", "Joana")}");
Console.WriteLine($"  com princípios: {ComPrincipios.FabricaDeServicoDeConfirmacao.Criar(entregador).Confirmar("Dom Casmurro", "Joana")}");

Console.WriteLine();
Console.WriteLine("Tamanho de cada versão (tipos e linhas com código):");
Imprimir("Responsabilidade, antes", MedidorDeTamanhoDoTopico.Medir("Topico11.Responsabilidade.Antes", "topico-11-solid/Responsabilidade/Antes"));
Imprimir("Responsabilidade, depois", MedidorDeTamanhoDoTopico.Medir("Topico11.Responsabilidade.Depois", "topico-11-solid/Responsabilidade/Depois"));
Imprimir("Inversão, antes", MedidorDeTamanhoDoTopico.Medir("Topico11.Inversao.Antes", "topico-11-solid/Inversao/Antes"));
Imprimir("Inversão, depois", MedidorDeTamanhoDoTopico.Medir("Topico11.Inversao.Depois", "topico-11-solid/Inversao/Depois"));
Imprimir("Confirmação, simples", MedidorDeTamanhoDoTopico.Medir("Topico11.Excesso.Simples", "topico-11-solid/Excesso/Simples"));
Imprimir("Confirmação, princípios", MedidorDeTamanhoDoTopico.Medir("Topico11.Excesso.ComPrincipios", "topico-11-solid/Excesso/ComPrincipios"));

static void Imprimir(string rotulo, Tamanho t) =>
    Console.WriteLine($"  {rotulo,-26} {t.Tipos} tipo(s), {t.Linhas,3} linhas");
