namespace Topico01;

// Estilo procedural: a função recebe os dados soltos e devolve um resultado.
// Nada impede quem chama de passar um prazo sem sentido.
public static class EmprestimoProcedural
{
    // Soma o prazo à data de retirada.
    public static DateOnly CalcularDevolucao(DateOnly retirada, int prazoEmDias)
        => retirada.AddDays(prazoEmDias);
}
