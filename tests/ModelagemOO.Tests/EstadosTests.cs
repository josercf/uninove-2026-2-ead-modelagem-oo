// Testes do tópico 08: o CicloDoEmprestimo contra estados.puml, transição a transição.
using System.Text.RegularExpressions;
using Topico03;
using Topico04;
using Topico06;
using Topico07;
using Topico08;

public class EstadosTests
{
    // Livro com prazo de 15 dias: retirado em 05/10/2026, a devolução prevista é 20/10/2026.
    private static readonly DateOnly Retirada = new(2026, 10, 5);
    private static readonly DateOnly NoPrazo = new(2026, 10, 18);
    private static readonly DateOnly DepoisDoPrazo = new(2026, 10, 21);

    private static (CicloDoEmprestimo Ciclo, Usuario Usuario, Livro Livro) NovoCiclo()
    {
        var usuario = new Usuario("Joana", Perfil.Morador);
        var livro = new Livro("Dom Casmurro", "Machado de Assis");
        var ciclo = new CicloDoEmprestimo(new Emprestimo(usuario, livro, Retirada), new ParametrosDaBiblioteca(1.50m));
        return (ciclo, usuario, livro);
    }

    // Leva um ciclo novo até o estado pedido pelos eventos do próprio código.
    private static CicloDoEmprestimo NoEstado(EstadoDoEmprestimo estado)
    {
        var (ciclo, _, _) = NovoCiclo();
        if (estado == EstadoDoEmprestimo.Atrasado) ciclo.VerificarPrazo(DepoisDoPrazo);
        if (estado == EstadoDoEmprestimo.Devolvido) ciclo.Devolver(NoPrazo);
        Assert.Equal(estado, ciclo.Estado);
        return ciclo;
    }

    private static readonly string[] Eventos = ["VerificarPrazo", "Renovar", "Devolver"];

    // Aplica o evento com a guarda verdadeira para o estado em que o ciclo está.
    private static void Aplicar(CicloDoEmprestimo ciclo, string evento)
    {
        switch (evento)
        {
            case "VerificarPrazo": ciclo.VerificarPrazo(DepoisDoPrazo); break;
            case "Renovar": ciclo.Renovar(NoPrazo); break;
            default: ciclo.Devolver(NoPrazo); break;
        }
    }

    // ---- Transições válidas ----

    [Fact]
    public void EmprestimoNovoNasceAtivoComAPrevistaDoEmprestimo()
    {
        var (ciclo, _, _) = NovoCiclo();
        Assert.Equal(EstadoDoEmprestimo.Ativo, ciclo.Estado);
        Assert.Equal(new DateOnly(2026, 10, 20), ciclo.Prevista);
        Assert.Empty(ciclo.Historico);
    }

    [Fact]
    public void AtivoVaiParaAtrasadoQuandoHojeEhDepoisDaPrevista()
    {
        var (ciclo, _, _) = NovoCiclo();
        Assert.True(ciclo.VerificarPrazo(DepoisDoPrazo));
        Assert.Equal(EstadoDoEmprestimo.Atrasado, ciclo.Estado);
        Assert.Equal(["Ativo --> Atrasado : VerificarPrazo"], ciclo.Historico);
    }

    [Fact]
    public void AtivoRenovadoContinuaAtivoComPrevistaNova()
    {
        var (ciclo, _, _) = NovoCiclo();
        ciclo.Renovar(NoPrazo);
        Assert.Equal(EstadoDoEmprestimo.Ativo, ciclo.Estado);
        // 18/10/2026 mais os 15 dias do livro.
        Assert.Equal(new DateOnly(2026, 11, 2), ciclo.Prevista);
        Assert.Equal(1, ciclo.Renovacoes);
    }

    [Fact]
    public void AtivoDevolvidoNoPrazoNaoGeraMulta()
    {
        var (ciclo, usuario, livro) = NovoCiclo();
        var comprovante = ciclo.Devolver(NoPrazo);
        Assert.Equal(EstadoDoEmprestimo.Devolvido, ciclo.Estado);
        Assert.Equal(0m, comprovante.Multa);
        Assert.False(usuario.TemMultaEmAberto);
        Assert.True(livro.Disponivel);
    }

    [Fact]
    public void AtrasadoDevolvidoGeraMultaPorDia()
    {
        var (ciclo, usuario, livro) = NovoCiclo();
        ciclo.VerificarPrazo(DepoisDoPrazo);
        var comprovante = ciclo.Devolver(new DateOnly(2026, 10, 23));
        Assert.Equal(EstadoDoEmprestimo.Devolvido, ciclo.Estado);
        Assert.Equal(3, comprovante.DiasDeAtraso);
        Assert.Equal(4.50m, comprovante.Multa);
        Assert.True(usuario.TemMultaEmAberto);
        Assert.True(livro.Disponivel);
    }

    // ---- Guardas ----

    [Fact]
    public void GuardaFalsaMantemAtivoSemTransicao()
    {
        var (ciclo, _, _) = NovoCiclo();
        // Exatamente na data prevista: hoje > prevista é falso.
        Assert.False(ciclo.VerificarPrazo(new DateOnly(2026, 10, 20)));
        Assert.Equal(EstadoDoEmprestimo.Ativo, ciclo.Estado);
        Assert.Empty(ciclo.Historico);
    }

    [Fact]
    public void RenovarDepoisDoPrazoEhRecusadoPelaGuardaEMantemOEstado()
    {
        var (ciclo, _, _) = NovoCiclo();
        var recusa = Assert.Throws<InvalidOperationException>(() => ciclo.Renovar(DepoisDoPrazo));
        Assert.IsNotType<TransicaoProibidaException>(recusa);
        Assert.Equal(EstadoDoEmprestimo.Ativo, ciclo.Estado);
        Assert.Equal(0, ciclo.Renovacoes);
    }

    [Fact]
    public void SegundaRenovacaoEhRecusadaPeloContadorEMantemOEstado()
    {
        var (ciclo, _, _) = NovoCiclo();
        ciclo.Renovar(NoPrazo);
        var recusa = Assert.Throws<InvalidOperationException>(() => ciclo.Renovar(NoPrazo.AddDays(1)));
        Assert.IsNotType<TransicaoProibidaException>(recusa);
        Assert.Equal(EstadoDoEmprestimo.Ativo, ciclo.Estado);
        Assert.Equal(1, ciclo.Renovacoes);
    }

    [Fact]
    public void EstadoAtrasadoSoExisteDepoisDeVerificarPrazo()
    {
        var (ciclo, _, _) = NovoCiclo();
        // Passou da data prevista, mas ninguém chamou VerificarPrazo: o enum ainda diz Ativo.
        Assert.Equal(EstadoDoEmprestimo.Ativo, ciclo.Estado);
        var comprovante = ciclo.Devolver(DepoisDoPrazo);
        Assert.Equal(1, comprovante.DiasDeAtraso);
        Assert.Equal(["Ativo --> Devolvido : Devolver"], ciclo.Historico);
    }

    // ---- Transições proibidas ----

    [Theory]
    [InlineData(EstadoDoEmprestimo.Atrasado, "VerificarPrazo")]
    [InlineData(EstadoDoEmprestimo.Atrasado, "Renovar")]
    [InlineData(EstadoDoEmprestimo.Devolvido, "VerificarPrazo")]
    [InlineData(EstadoDoEmprestimo.Devolvido, "Renovar")]
    [InlineData(EstadoDoEmprestimo.Devolvido, "Devolver")]
    public void TransicaoProibidaLancaExcecaoEMantemOEstado(EstadoDoEmprestimo estado, string evento)
    {
        var ciclo = NoEstado(estado);
        var historicoAntes = ciclo.Historico.ToList();
        var recusa = Assert.Throws<TransicaoProibidaException>(() => Aplicar(ciclo, evento));
        Assert.Equal(estado, recusa.Estado);
        Assert.Equal(evento, recusa.Evento);
        Assert.Equal($"Transição proibida: {estado} não aceita {evento}.", recusa.Message);
        // A recusa não deixa rastro: estado e histórico seguem como estavam.
        Assert.Equal(estado, ciclo.Estado);
        Assert.Equal(historicoAntes, ciclo.Historico);
    }

    [Fact]
    public void DevolverDuasVezesNaoMexeNoItemNemNoUsuario()
    {
        var (ciclo, usuario, livro) = NovoCiclo();
        ciclo.Devolver(NoPrazo);
        Assert.Throws<TransicaoProibidaException>(() => ciclo.Devolver(DepoisDoPrazo));
        Assert.False(usuario.TemMultaEmAberto);
        Assert.True(livro.Disponivel);
    }

    // ---- Conferência contra estados.puml ----

    private static string LerDiagrama()
    {
        var pasta = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(pasta.FullName, "ModelagemOO.sln")))
            pasta = pasta.Parent ?? throw new FileNotFoundException("ModelagemOO.sln não encontrado.");
        return File.ReadAllText(Path.Combine(pasta.FullName, "topico-08-estados-componentes-implantacao", "estados.puml"));
    }

    // Setas "Origem --> Destino : Evento [guarda]"; as setas de [*] não têm evento e ficam de fora.
    private static List<(string Origem, string Destino, string Evento, string? Guarda)> SetasDoDiagrama() =>
        Regex.Matches(LerDiagrama(), @"^(\w+) --> (\w+) : (\w+)(?: \[(.+)\])?\s*$", RegexOptions.Multiline)
            .Select(m => (m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value,
                m.Groups[4].Success ? m.Groups[4].Value : null))
            .ToList();

    private static List<(string Origem, string Evento)> ProibidasDoDiagrama() =>
        Regex.Matches(LerDiagrama(), @"^' proibida: (\w+) : (\w+)\s*$", RegexOptions.Multiline)
            .Select(m => (m.Groups[1].Value, m.Groups[2].Value))
            .ToList();

    [Fact]
    public void OsEventosAceitosPeloCodigoSaoAsSetasDeEstadosPuml()
    {
        // Roda cada evento em cada estado, com a guarda verdadeira, e anota o destino que o código produz.
        var doCodigo = new List<(string, string, string)>();
        foreach (var estado in Enum.GetValues<EstadoDoEmprestimo>())
            foreach (var evento in Eventos)
            {
                var ciclo = NoEstado(estado);
                try
                {
                    Aplicar(ciclo, evento);
                    doCodigo.Add((estado.ToString(), ciclo.Estado.ToString(), evento));
                }
                catch (TransicaoProibidaException) { }
            }
        var doDiagrama = SetasDoDiagrama().Select(s => (s.Origem, s.Destino, s.Evento)).ToList();
        Assert.True(doDiagrama.Count >= 4, $"só {doDiagrama.Count} setas lidas de estados.puml");
        Assert.Equal(doDiagrama.OrderBy(t => t).ToList(), doCodigo.OrderBy(t => t).ToList());
    }

    [Fact]
    public void AsRecusasDoCodigoSaoAsLinhasProibidasDeEstadosPuml()
    {
        var doCodigo = new List<(string, string)>();
        foreach (var estado in Enum.GetValues<EstadoDoEmprestimo>())
            foreach (var evento in Eventos)
            {
                try { Aplicar(NoEstado(estado), evento); }
                catch (TransicaoProibidaException) { doCodigo.Add((estado.ToString(), evento)); }
            }
        var doDiagrama = ProibidasDoDiagrama();
        Assert.Equal(5, doDiagrama.Count);
        Assert.Equal(doDiagrama.OrderBy(t => t).ToList(), doCodigo.OrderBy(t => t).ToList());
    }

    [Fact]
    public void CadaCelulaDaTabelaEstadoPorEventoEstaNoDiagramaComoSetaOuComoProibida()
    {
        // 3 estados vezes 3 eventos: 4 setas e 5 proibidas cobrem as 9 células.
        var celulas = SetasDoDiagrama().Select(s => (s.Origem, s.Evento))
            .Concat(ProibidasDoDiagrama()).Distinct().ToList();
        var esperado = from e in Enum.GetValues<EstadoDoEmprestimo>() from ev in Eventos select (e.ToString(), ev);
        Assert.Equal(esperado.OrderBy(t => t).ToList(), celulas.OrderBy(t => t).ToList());
    }

    [Fact]
    public void AsGuardasDoDiagramaDescrevemAsCondicoesDoCodigo()
    {
        var setas = SetasDoDiagrama();
        Assert.Equal("hoje > prevista", setas.Single(s => s.Evento == "VerificarPrazo").Guarda);
        Assert.Equal($"hoje <= prevista e renovacoes < {CicloDoEmprestimo.MaximoDeRenovacoes}",
            setas.Single(s => s.Evento == "Renovar").Guarda);
        Assert.All(setas.Where(s => s.Evento == "Devolver"), s => Assert.Null(s.Guarda));
    }

    [Fact]
    public void OHistoricoDeUmCicloCompletoEhUmCaminhoDeEstadosPuml()
    {
        var (ciclo, _, _) = NovoCiclo();
        ciclo.Renovar(NoPrazo);
        ciclo.VerificarPrazo(new DateOnly(2026, 11, 3));
        ciclo.Devolver(new DateOnly(2026, 11, 5));
        var setas = SetasDoDiagrama().Select(s => $"{s.Origem} --> {s.Destino} : {s.Evento}").ToHashSet();
        Assert.Equal(3, ciclo.Historico.Count);
        Assert.All(ciclo.Historico, t => Assert.Contains(t, setas));
    }
}
