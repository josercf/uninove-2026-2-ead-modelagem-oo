namespace Topico08;

// Recusa de um evento que o estado atual não aceita (transição proibida em estados.puml).
public class TransicaoProibidaException(EstadoDoEmprestimo estado, string evento)
    : InvalidOperationException($"Transição proibida: {estado} não aceita {evento}.")
{
    public EstadoDoEmprestimo Estado { get; } = estado;
    public string Evento { get; } = evento;
}
