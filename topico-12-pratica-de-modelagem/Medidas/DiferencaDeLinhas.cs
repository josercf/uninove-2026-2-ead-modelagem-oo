namespace Topico12.Medidas;

// Contagem de linhas acrescentadas e removidas entre duas versões de um arquivo de texto.
public record DiferencaDeLinhas(int Acrescentadas, int Removidas)
{
    // Maior subsequência comum de linhas: o que sobra de cada lado é o que mudou.
    // Linha em branco não conta; uma linha editada conta como uma removida e uma acrescentada.
    public static DiferencaDeLinhas Contar(string antes, string depois)
    {
        var a = Linhas(antes);
        var b = Linhas(depois);
        var lcs = new int[a.Count + 1, b.Count + 1];
        for (var i = a.Count - 1; i >= 0; i--)
            for (var j = b.Count - 1; j >= 0; j--)
                lcs[i, j] = a[i] == b[j] ? lcs[i + 1, j + 1] + 1 : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);
        var comuns = lcs[0, 0];
        return new(b.Count - comuns, a.Count - comuns);
    }

    private static List<string> Linhas(string texto) =>
        texto.Split('\n').Select(l => l.TrimEnd()).Where(l => l != "").ToList();
}
