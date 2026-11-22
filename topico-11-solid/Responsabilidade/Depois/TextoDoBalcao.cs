using System.Globalization;

namespace Topico11.Responsabilidade.Depois;

// Única razão de mudança: o texto que o balcão mostra e registra.
public static class TextoDoBalcao
{
    private static readonly CultureInfo Pt = CultureInfo.GetCultureInfo("pt-BR");

    [MotivoDeMudanca("texto das mensagens")]
    public static string Confirmacao(string titulo, string nome) =>
        $"Empréstimo de \"{titulo}\" registrado para {nome}.";

    [MotivoDeMudanca("texto das mensagens")]
    public static string LinhaDeEmprestimo(string titulo, string nome, bool aceito) =>
        $"Empréstimo de {titulo} para {nome}: {(aceito ? "aceito" : "recusado")}";

    [MotivoDeMudanca("texto das mensagens")]
    public static string LinhaDeDevolucao(string titulo, int dias, decimal multa) =>
        $"Devolução de {titulo}: {dias} dia(s), multa R$ {multa.ToString("0.00", Pt)}";
}
