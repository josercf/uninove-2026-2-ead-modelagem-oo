using Topico06;
using Topico07;

namespace Topico08;

// Máquina de estados de um Emprestimo do tópico 06: Ativo, Atrasado e Devolvido.
// Cada método público é um evento e corresponde a uma seta de estados.puml.
public class CicloDoEmprestimo
{
    // Limite de renovações: dado ao lado do estado, não um estado novo.
    public const int MaximoDeRenovacoes = 1;

    private readonly Emprestimo _emprestimo;
    private readonly ParametrosDaBiblioteca _parametros;

    public CicloDoEmprestimo(Emprestimo emprestimo, ParametrosDaBiblioteca parametros)
    {
        ArgumentNullException.ThrowIfNull(emprestimo);
        ArgumentNullException.ThrowIfNull(parametros);
        _emprestimo = emprestimo;
        _parametros = parametros;
        // Estado inicial: a seta que sai do círculo preto do diagrama.
        Estado = EstadoDoEmprestimo.Ativo;
        Prevista = emprestimo.Devolucao;
    }

    public EstadoDoEmprestimo Estado { get; private set; }
    public DateOnly Prevista { get; private set; }
    public int Renovacoes { get; private set; }

    // Cada transição executada, no mesmo formato das setas de estados.puml.
    public List<string> Historico { get; } = [];

    // Ativo --> Atrasado, com guarda "hoje > prevista". Guarda falsa mantém o estado e devolve false.
    public bool VerificarPrazo(DateOnly hoje)
    {
        if (Estado != EstadoDoEmprestimo.Ativo)
            throw new TransicaoProibidaException(Estado, nameof(VerificarPrazo));
        if (hoje <= Prevista)
            return false;
        Mover(EstadoDoEmprestimo.Atrasado, nameof(VerificarPrazo));
        return true;
    }

    // Ativo --> Ativo, com guarda "hoje <= prevista e renovacoes < 1".
    public void Renovar(DateOnly hoje)
    {
        if (Estado != EstadoDoEmprestimo.Ativo)
            throw new TransicaoProibidaException(Estado, nameof(Renovar));
        // Guarda falsa: o estado é o certo, mas a condição não vale, e nada muda.
        if (hoje > Prevista)
            throw new InvalidOperationException("O prazo já venceu: a renovação exige empréstimo dentro do prazo.");
        if (Renovacoes >= MaximoDeRenovacoes)
            throw new InvalidOperationException($"O empréstimo já foi renovado {MaximoDeRenovacoes} vez.");
        Renovacoes++;
        Prevista = hoje.AddDays(_emprestimo.Item.PrazoEmDias);
        Mover(EstadoDoEmprestimo.Ativo, nameof(Renovar));
    }

    // Ativo --> Devolvido e Atrasado --> Devolvido: o mesmo evento vale nos dois estados.
    public ComprovanteDeDevolucao Devolver(DateOnly entrega)
    {
        if (Estado == EstadoDoEmprestimo.Devolvido)
            throw new TransicaoProibidaException(Estado, nameof(Devolver));
        var origem = Estado;
        _emprestimo.Item.Devolver();
        // A multa vem dos dias entre a data prevista e a entrega, medidos nesta hora.
        var dias = Math.Max(0, entrega.DayNumber - Prevista.DayNumber);
        var multa = dias * _parametros.MultaPorDia;
        if (dias > 0)
            _emprestimo.Usuario.RegistrarMulta();
        Mover(EstadoDoEmprestimo.Devolvido, nameof(Devolver), origem);
        return new ComprovanteDeDevolucao(_emprestimo.Item.Titulo, dias, multa);
    }

    // Troca o estado e registra a transição no formato do diagrama.
    private void Mover(EstadoDoEmprestimo destino, string evento, EstadoDoEmprestimo? origem = null)
    {
        Historico.Add($"{origem ?? Estado} --> {destino} : {evento}");
        Estado = destino;
    }
}
