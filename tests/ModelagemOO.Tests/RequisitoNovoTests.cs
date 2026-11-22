// Testes do tópico 12: um requisito novo (reserva bloqueia renovação) atravessando quatro diagramas.
using System.Text.RegularExpressions;
using Topico03;
using Topico04;
using Topico05;
using Topico06;
using Topico07;
using Topico08;
using Topico12.Geracao;
using Topico12.Medidas;
using Topico12.Requisito;

public class RequisitoNovoTests
{
    private static readonly DateOnly Retirada = new(2026, 10, 5);
    private static readonly DateOnly NoPrazo = new(2026, 10, 18);

    private static string Depois(string arquivo) =>
        RepositorioParaTestes.Ler("topico-12-pratica-de-modelagem", "Requisito", "Depois", arquivo);

    private static (RenovacaoComReserva Renovacao, FilaDeReservas Fila, CicloDoEmprestimo Ciclo, Livro Livro, Rastro Rastro) Cenario()
    {
        var joana = new Usuario("Joana", Perfil.Estudante);
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        var emprestimo = new Emprestimo(joana, livro, Retirada);
        var ciclo = new CicloDoEmprestimo(emprestimo, new ParametrosDaBiblioteca(1.50m));
        var fila = new FilaDeReservas();
        var rastro = new Rastro();
        return (new RenovacaoComReserva(ciclo, emprestimo, fila, rastro), fila, ciclo, livro, rastro);
    }

    // ---------- comportamento do requisito ----------

    [Fact]
    public void SemReservaARenovacaoSegueOCicloDoTopico08()
    {
        var (renovacao, _, ciclo, _, _) = Cenario();
        renovacao.Renovar(NoPrazo);
        Assert.Equal(EstadoDoEmprestimo.Ativo, ciclo.Estado);
        Assert.Equal(1, ciclo.Renovacoes);
    }

    [Fact]
    public void ComReservaARenovacaoEhRecusadaEOEstadoNaoMuda()
    {
        var (renovacao, fila, ciclo, livro, _) = Cenario();
        fila.Reservar(new Usuario("Caio", Perfil.Morador), livro, NoPrazo);
        Assert.Throws<RenovacaoBloqueadaPorReservaException>(() => renovacao.Renovar(NoPrazo));
        Assert.Equal(EstadoDoEmprestimo.Ativo, ciclo.Estado);
        Assert.Equal(0, ciclo.Renovacoes);
        // A recusa não acrescenta transição ao histórico.
        Assert.Empty(ciclo.Historico);
    }

    [Fact]
    public void ItemDisponivelNaoSeReserva()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        var fila = new FilaDeReservas();
        // Item na estante se retira no balcão (UC01); reserva só existe para item emprestado.
        Assert.Throws<InvalidOperationException>(() => fila.Reservar(new Usuario("Caio", Perfil.Morador), livro, NoPrazo));
    }

    [Fact]
    public void ReservaDeOutroItemNaoBloqueiaARenovacao()
    {
        var (renovacao, fila, ciclo, _, _) = Cenario();
        var revista = new Revista("Superinteressante", 480);
        revista.Emprestar(Retirada);
        fila.Reservar(new Usuario("Caio", Perfil.Morador), revista, NoPrazo);
        renovacao.Renovar(NoPrazo);
        Assert.Equal(1, ciclo.Renovacoes);
    }

    // ---------- sequência: o rastro contra sequencia.puml (versão depois) ----------

    // Lê as setas de sequencia.puml em três grupos: antes do alt, ramo "então" e ramo "senão".
    private static (List<string> Comum, List<string> Ramo1, List<string> Ramo2) SetasDoDiagrama()
    {
        var comum = new List<string>();
        var ramo1 = new List<string>();
        var ramo2 = new List<string>();
        var atual = comum;
        // O diagrama novo guarda o cenário do tópico 07 e acrescenta a renovação depois do divisor "== Renovação".
        var depoisDoDivisor = false;
        foreach (var linha in Depois("sequencia.puml").Split('\n').Select(l => l.Trim()))
        {
            if (linha.StartsWith("== Renovação")) depoisDoDivisor = true;
            else if (!depoisDoDivisor) continue;
            else if (linha.StartsWith("alt ")) atual = ramo1;
            else if (linha.StartsWith("else")) atual = ramo2;
            else if (Regex.IsMatch(linha, @"^\w+ --?> \w+ : .+$"))
                atual.Add(linha);
        }
        return (comum, ramo1, ramo2);
    }

    [Fact]
    public void RastroDaRenovacaoBloqueadaSegueORamoComReservaDoDiagrama()
    {
        var (renovacao, fila, _, livro, rastro) = Cenario();
        fila.Reservar(new Usuario("Caio", Perfil.Morador), livro, NoPrazo);
        Assert.Throws<RenovacaoBloqueadaPorReservaException>(() => renovacao.Renovar(NoPrazo));
        var (comum, ramo1, _) = SetasDoDiagrama();
        Assert.Equal([.. comum, .. ramo1], rastro.Mensagens);
    }

    [Fact]
    public void RastroDaRenovacaoLiberadaSegueORamoSemReservaDoDiagrama()
    {
        var (renovacao, _, _, _, rastro) = Cenario();
        renovacao.Renovar(NoPrazo);
        var (comum, _, ramo2) = SetasDoDiagrama();
        Assert.Equal([.. comum, .. ramo2], rastro.Mensagens);
    }

    // ---------- estados: a guarda nova ----------

    [Fact]
    public void EstadosDepoisTemUmaSetaRenovarComGuardaDeReserva()
    {
        var setas = Regex.Matches(Depois("estados.puml"), @"^(\w+) --> (\w+) : (\w+)(?: \[(.+)\])?\s*$", RegexOptions.Multiline)
            .Select(m => (De: m.Groups[1].Value, Para: m.Groups[2].Value, Evento: m.Groups[3].Value, Guarda: m.Groups[4].Value))
            .ToList();
        // O diagrama continua com as 4 setas aceitas do tópico 08; só a guarda de Renovar muda.
        Assert.Equal(4, setas.Count);
        var renovar = Assert.Single(setas, s => s.Evento == "Renovar");
        Assert.Equal("hoje <= prevista e renovacoes < 1 e sem reserva", renovar.Guarda);
    }

    // ---------- classes: o diagrama novo contra o código novo ----------

    [Fact]
    public void ClassesDepoisNaoDivergeDoCodigoComAsTresClassesNovas()
    {
        Type[] tipos =
        [
            typeof(ItemAcervo), typeof(Livro), typeof(Revista), typeof(Midia),
            typeof(Usuario), typeof(Perfil), typeof(Balcao), typeof(Emprestimo),
            typeof(Reserva), typeof(FilaDeReservas), typeof(RenovacaoComReserva),
        ];
        var divergencias = ComparadorDeModelo.Comparar(Depois("classes.puml"), GeradorDeDiagramaDeClasses.Gerar(tipos));
        Assert.Empty(divergencias);
    }

    // ---------- casos de uso: estrutura do diagrama novo ----------

    [Fact]
    public void CasosDeUsoDepoisAcrescentaUC07ComIncludeDeUC08()
    {
        var uc = Depois("casos-de-uso.puml");
        Assert.Equal(8, Regex.Matches(uc, @"^\s*usecase ""UC\d\d\\n", RegexOptions.Multiline).Count);
        Assert.Contains("Atendente -- UC07", uc);
        Assert.Contains("UC07 ..> UC08 : <<include>>", uc);
    }

    // ---------- contagem de diagramas e de linhas alteradas ----------

    private static readonly (string Antes, string Depois)[] Pares =
    [
        ("topico-05-casos-de-uso/casos-de-uso.puml", "casos-de-uso.puml"),
        ("topico-06-classes-e-objetos/classes.puml", "classes.puml"),
        ("topico-07-sequencia-e-atividades/sequencia.puml", "sequencia.puml"),
        ("topico-08-estados-componentes-implantacao/estados.puml", "estados.puml"),
    ];

    [Fact]
    public void RequisitoAlteraQuatroDosOitoDiagramasEAsLinhasMedidasSaoAsDoDiff()
    {
        var medidas = Pares.Select(p => (p.Depois, Diff: DiferencaDeLinhas.Contar(
            RepositorioParaTestes.Ler(p.Antes.Split('/')), Depois(p.Depois)))).ToList();
        // Linhas acrescentadas e removidas por diagrama, sem contar linha em branco; medido por dotnet run.
        Assert.Equal(new[] { ("casos-de-uso.puml", 5, 0), ("classes.puml", 22, 1), ("sequencia.puml", 15, 0), ("estados.puml", 6, 1) },
            medidas.Select(m => (m.Depois, m.Diff.Acrescentadas, m.Diff.Removidas)).ToArray());
        Assert.Equal(50, medidas.Sum(m => m.Diff.Acrescentadas + m.Diff.Removidas));
        // Os outros quatro diagramas (objetos, atividades, componentes, implantação) não têm versão nova.
        Assert.Equal(8, InventarioDeDiagramas.Ler(RepositorioParaTestes.Raiz()).Arquivos.Count);
    }

    [Fact]
    public void ContadorDeLinhasIgnoraLinhaEmBrancoEContaTrocaComoRemocaoMaisAcrescimo()
    {
        var d = DiferencaDeLinhas.Contar("a\nb\n\nc", "a\nB\nc\nd\n");
        Assert.Equal(2, d.Acrescentadas);
        Assert.Equal(1, d.Removidas);
    }
}
