// Testes do tópico 07: o rastro das chamadas contra sequencia.puml e as ações contra atividades.puml.
using System.Text.RegularExpressions;
using Topico03;
using Topico04;
using Topico06;
using Topico07;

public class SequenciaEAtividadesTests
{
    private static readonly DateOnly Retirada = new(2026, 10, 5);
    private static readonly ParametrosDaBiblioteca Parametros = new(1.50m);

    private static (BalcaoComRastro Balcao, Rastro Rastro) NovoBalcao()
    {
        var rastro = new Rastro();
        return (new BalcaoComRastro(rastro, Parametros), rastro);
    }

    private static Livro NovoLivro() => new("Dom Casmurro", "Machado de Assis");

    // ---- Ordem das mensagens ----

    [Fact]
    public void UmItemAceitoGeraAsOitoMensagensNaOrdemDoDiagrama()
    {
        var (balcao, rastro) = NovoBalcao();
        var resultados = balcao.EmprestarSacola(new Usuario("Joana", Perfil.Morador), [NovoLivro()], Retirada);
        Assert.True(resultados.Single().Aceito);
        // Chamadas com seta cheia e retornos com seta tracejada, na ordem em que aconteceram.
        Assert.Equal(
        [
            "Atendente -> Balcao : EmprestarSacola(usuario, itens, retirada)",
            "Balcao -> ItemAcervo : Disponivel",
            "ItemAcervo --> Balcao : bool",
            "Balcao -> Usuario : Retirar(titulo)",
            "Usuario --> Balcao : ok",
            "Balcao -> ItemAcervo : Emprestar(retirada)",
            "ItemAcervo --> Balcao : devolucao",
            "Balcao --> Atendente : resultados",
        ], rastro.Mensagens);
        AssertCaminhoDoDiagrama(rastro.Mensagens);
    }

    [Fact]
    public void SacolaDeTresItensParaMoradorTerminaNaRecusaPeloLimite()
    {
        var (balcao, rastro) = NovoBalcao();
        var joana = new Usuario("Joana", Perfil.Morador);
        var resultados = balcao.EmprestarSacola(joana,
            [NovoLivro(), new Revista("Superinteressante", 480), new Midia("Central do Brasil", "DVD")], Retirada);
        Assert.Equal([true, true, false], resultados.Select(r => r.Aceito));
        Assert.Equal("Usuário atingiu o Limite de 2 itens do perfil Morador.", resultados[2].Mensagem);
        Assert.Contains("Usuario --> Balcao : InvalidOperationException", rastro.Mensagens);
        AssertCaminhoDoDiagrama(rastro.Mensagens);
    }

    [Fact]
    public void ItemJaEmprestadoNaoChegaAoUsuario()
    {
        var (balcao, rastro) = NovoBalcao();
        var livro = NovoLivro();
        livro.Emprestar(Retirada);
        var resultados = balcao.EmprestarSacola(new Usuario("Caio", Perfil.Estudante), [livro], Retirada);
        Assert.False(resultados.Single().Aceito);
        // A recusa do item acontece antes de qualquer mensagem ao Usuario.
        Assert.DoesNotContain(rastro.Mensagens, m => m.Contains("Usuario"));
        AssertCaminhoDoDiagrama(rastro.Mensagens);
    }

    [Fact]
    public void MultaEmAbertoRecusaNoUsuarioEDeixaOItemLivre()
    {
        var (balcao, rastro) = NovoBalcao();
        var caio = new Usuario("Caio", Perfil.Estudante);
        caio.RegistrarMulta();
        var livro = NovoLivro();
        var resultados = balcao.EmprestarSacola(caio, [livro], Retirada);
        Assert.Equal("Usuário com multa em aberto não retira item novo.", resultados.Single().Mensagem);
        // Retirar recusou antes de Emprestar: o item continua disponível.
        Assert.True(livro.Disponivel);
        Assert.DoesNotContain(rastro.Mensagens, m => m.Contains("Emprestar(retirada)"));
        AssertCaminhoDoDiagrama(rastro.Mensagens);
    }

    [Fact]
    public void SacolaVaziaPercorreOLoopZeroVezes()
    {
        var (balcao, rastro) = NovoBalcao();
        balcao.EmprestarSacola(new Usuario("Joana", Perfil.Morador), [], Retirada);
        Assert.Equal(
        [
            "Atendente -> Balcao : EmprestarSacola(usuario, itens, retirada)",
            "Balcao --> Atendente : resultados",
        ], rastro.Mensagens);
        AssertCaminhoDoDiagrama(rastro.Mensagens);
    }

    [Fact]
    public void RastroForaDaOrdemDoDiagramaEReprovado()
    {
        var (balcao, rastro) = NovoBalcao();
        balcao.EmprestarSacola(new Usuario("Joana", Perfil.Morador), [NovoLivro()], Retirada);
        var trocado = rastro.Mensagens.ToList();
        // Troca Retirar por Emprestar: o diagrama não admite essa ordem.
        (trocado[3], trocado[5]) = (trocado[5], trocado[3]);
        Assert.False(DiagramaAceita(trocado));
        // Um rastro interrompido no meio do alt também não completa o diagrama.
        Assert.False(DiagramaAceita(rastro.Mensagens.Take(4).ToList()));
    }

    [Fact]
    public void OsCenariosJuntosPercorremTodasAsMensagensDoDiagrama()
    {
        var (balcao, rastro) = NovoBalcao();
        var joana = new Usuario("Joana", Perfil.Morador);
        var emprestado = NovoLivro();
        emprestado.Emprestar(Retirada);
        var comMulta = new Usuario("Caio", Perfil.Estudante);
        comMulta.RegistrarMulta();
        balcao.EmprestarSacola(joana, [new Revista("Superinteressante", 480), new Livro("O Cortiço", "Aluísio Azevedo"), new Midia("Central do Brasil", "DVD")], Retirada);
        balcao.EmprestarSacola(joana, [emprestado], Retirada);
        balcao.EmprestarSacola(comMulta, [new Livro("Capitães da Areia", "Jorge Amado")], Retirada);
        var doDiagrama = MensagensDoDiagrama().Select(m => m.Texto).ToHashSet();
        // Cada mensagem desenhada acontece em algum cenário, e nenhuma mensagem do código falta no desenho.
        Assert.Equal(doDiagrama, rastro.Mensagens.ToHashSet());
        Assert.Equal(9, doDiagrama.Count);
    }

    [Fact]
    public void EmprestarSacolaDaOMesmoResultadoQueOConstrutorDeEmprestimo()
    {
        var (balcao, _) = NovoBalcao();
        var viaBalcao = balcao.EmprestarSacola(new Usuario("Joana", Perfil.Morador), [new Revista("Superinteressante", 480)], Retirada).Single();
        var viaEmprestimo = new Emprestimo(new Usuario("Ana", Perfil.Morador), new Revista("Superinteressante", 480), Retirada);
        Assert.Equal(viaEmprestimo.Devolucao, viaBalcao.Devolucao);
        // As recusas usam o mesmo texto do construtor de Emprestimo do tópico 06.
        var ocupado = NovoLivro();
        ocupado.Emprestar(Retirada);
        var recusaBalcao = balcao.EmprestarSacola(new Usuario("Caio", Perfil.Estudante), [ocupado], Retirada).Single();
        var recusaEmprestimo = Assert.Throws<InvalidOperationException>(() => new Emprestimo(new Usuario("Caio", Perfil.Estudante), ocupado, Retirada));
        Assert.Equal(recusaEmprestimo.Message, recusaBalcao.Mensagem);
    }

    // ---- Atividades ----

    [Fact]
    public void DevolucaoNoPrazoNaoCalculaMulta()
    {
        var (balcao, rastro) = NovoBalcao();
        var joana = new Usuario("Joana", Perfil.Morador);
        var livro = NovoLivro();
        var devolucao = livro.Emprestar(Retirada);
        var comprovante = balcao.Devolver(joana, livro, devolucao, devolucao);
        Assert.Equal(0, comprovante.DiasDeAtraso);
        Assert.Equal(0m, comprovante.Multa);
        Assert.False(joana.TemMultaEmAberto);
        Assert.True(livro.Disponivel);
        Assert.Equal(["Receber o item no balcão", "Marcar o item como disponível", "Emitir o comprovante"], rastro.Acoes);
    }

    [Fact]
    public void DevolucaoComTresDiasDeAtrasoCobraTresVezesOParametro()
    {
        var (balcao, rastro) = NovoBalcao();
        var joana = new Usuario("Joana", Perfil.Morador);
        var livro = NovoLivro();
        var prevista = livro.Emprestar(Retirada);
        var comprovante = balcao.Devolver(joana, livro, prevista, prevista.AddDays(3));
        Assert.Equal(3, comprovante.DiasDeAtraso);
        Assert.Equal(4.50m, comprovante.Multa);
        Assert.True(joana.TemMultaEmAberto);
        // O novo empréstimo de Joana fica bloqueado pela multa (RF02).
        Assert.Throws<InvalidOperationException>(() => joana.Retirar("Outro livro"));
        Assert.Contains("Calcular a multa", rastro.Acoes);
        Assert.Contains("Registrar a multa no usuário", rastro.Acoes);
    }

    [Fact]
    public void AcoesDaDevolucaoSeguemOForkDoDiagramaDeAtividades()
    {
        var acoesDoDiagrama = AcoesDoDiagrama();
        var acoesDoCodigo = new HashSet<string>();
        foreach (var atraso in new[] { 0, 3 })
        {
            var (balcao, rastro) = NovoBalcao();
            var livro = NovoLivro();
            var prevista = livro.Emprestar(Retirada);
            balcao.Devolver(new Usuario("Joana", Perfil.Morador), livro, prevista, prevista.AddDays(atraso));
            // Antes do fork vem o recebimento e depois do join vem o comprovante, nos dois caminhos.
            Assert.Equal(acoesDoDiagrama[0], rastro.Acoes.First());
            Assert.Equal(acoesDoDiagrama[^1], rastro.Acoes.Last());
            acoesDoCodigo.UnionWith(rastro.Acoes);
        }
        // Cada ação desenhada acontece em algum dos dois caminhos, e o código não faz ação fora do desenho.
        Assert.Equal(acoesDoDiagrama.ToHashSet(), acoesDoCodigo);
        Assert.Equal(5, acoesDoDiagrama.Count);
    }

    [Fact]
    public void DevolverItemQueNaoEstaEmprestadoLancaExcecao()
    {
        var (balcao, _) = NovoBalcao();
        var livro = NovoLivro();
        Assert.Throws<InvalidOperationException>(() => balcao.Devolver(new Usuario("Joana", Perfil.Morador), livro, Retirada, Retirada));
    }

    [Fact]
    public void MultaPorDiaNegativaEReprovada()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BalcaoComRastro(new Rastro(), new ParametrosDaBiblioteca(-1m)));
    }

    // ---- Leitura dos diagramas ----

    private abstract record No;
    private record Msg(string Texto) : No;
    private record Alt(List<List<No>> Ramos) : No;
    private record Repete(List<No> Corpo) : No;

    private static string LerArquivo(string arquivo)
    {
        var pasta = new DirectoryInfo(AppContext.BaseDirectory);
        while (!File.Exists(Path.Combine(pasta.FullName, "ModelagemOO.sln")))
            pasta = pasta.Parent ?? throw new FileNotFoundException("ModelagemOO.sln não encontrado.");
        return File.ReadAllText(Path.Combine(pasta.FullName, "topico-07-sequencia-e-atividades", arquivo));
    }

    // Lê sequencia.puml e monta a árvore: mensagens, alt com ramos e loop com corpo.
    private static List<No> ArvoreDoDiagrama()
    {
        var raiz = new List<No>();
        // Cada quadro aberto (loop ou alt) guarda os ramos que já tem; o último ramo recebe as mensagens.
        var pilha = new Stack<(string Tipo, List<List<No>> Ramos)>();
        pilha.Push(("raiz", [raiz]));
        foreach (var bruta in LerArquivo("sequencia.puml").Split('\n'))
        {
            var linha = bruta.Trim();
            var atual = pilha.Peek().Ramos[^1];
            var mensagem = Regex.Match(linha, @"^(\w+) (-->|->) (\w+) : (.+)$");
            if (mensagem.Success)
                atual.Add(new Msg($"{mensagem.Groups[1].Value} {mensagem.Groups[2].Value} {mensagem.Groups[3].Value} : {mensagem.Groups[4].Value}"));
            else if (linha.StartsWith("loop ")) pilha.Push(("loop", [[]]));
            else if (linha.StartsWith("alt ")) pilha.Push(("alt", [[]]));
            else if (linha.StartsWith("else")) pilha.Peek().Ramos.Add([]);
            else if (linha == "end")
            {
                var (tipo, ramos) = pilha.Pop();
                pilha.Peek().Ramos[^1].Add(tipo == "loop" ? new Repete(ramos[0]) : new Alt(ramos));
            }
        }
        Assert.Single(pilha);
        return raiz;
    }

    private static List<Msg> MensagensDoDiagrama()
    {
        var todas = new List<Msg>();
        void Visitar(IEnumerable<No> nos)
        {
            foreach (var no in nos)
                switch (no)
                {
                    case Msg m: todas.Add(m); break;
                    case Alt a: foreach (var ramo in a.Ramos) Visitar(ramo); break;
                    case Repete r: Visitar(r.Corpo); break;
                }
        }
        Visitar(ArvoreDoDiagrama());
        // Guarda contra leitura pela metade: o diagrama desenha 9 mensagens.
        Assert.True(todas.Count >= 9, $"só {todas.Count} mensagens lidas de sequencia.puml");
        return todas;
    }

    // Devolve as posições do rastro em que o trecho do diagrama pode terminar, partindo de 'inicio'.
    private static HashSet<int> Casar(IEnumerable<No> nos, IReadOnlyList<string> rastro, HashSet<int> inicio)
    {
        var posicoes = inicio;
        foreach (var no in nos)
        {
            switch (no)
            {
                // Mensagem: a posição avança se a próxima mensagem do rastro é igual à desenhada.
                case Msg m:
                    posicoes = posicoes.Where(p => p < rastro.Count && rastro[p] == m.Texto).Select(p => p + 1).ToHashSet();
                    break;
                // Alt: vale qualquer um dos ramos.
                case Alt a:
                    posicoes = a.Ramos.SelectMany(ramo => Casar(ramo, rastro, posicoes)).ToHashSet();
                    break;
                // Loop: zero ou mais passagens pelo corpo, até não surgir posição nova.
                case Repete r:
                    var alcancadas = new HashSet<int>(posicoes);
                    var fronteira = posicoes;
                    while (fronteira.Count > 0)
                    {
                        fronteira = Casar(r.Corpo, rastro, fronteira).Where(p => !alcancadas.Contains(p)).ToHashSet();
                        alcancadas.UnionWith(fronteira);
                    }
                    posicoes = alcancadas;
                    break;
            }
        }
        return posicoes;
    }

    private static bool DiagramaAceita(IReadOnlyList<string> rastro) =>
        Casar(ArvoreDoDiagrama(), rastro, [0]).Contains(rastro.Count);

    private static void AssertCaminhoDoDiagrama(IReadOnlyList<string> rastro) =>
        Assert.True(DiagramaAceita(rastro), "o rastro não é um caminho de sequencia.puml:\n" + string.Join("\n", rastro));

    // Lê as ações de atividades.puml, linhas no formato ":texto;".
    private static List<string> AcoesDoDiagrama() =>
        Regex.Matches(LerArquivo("atividades.puml"), @"^\s*:(.+);\s*$", RegexOptions.Multiline)
            .Select(a => a.Groups[1].Value).ToList();
}
