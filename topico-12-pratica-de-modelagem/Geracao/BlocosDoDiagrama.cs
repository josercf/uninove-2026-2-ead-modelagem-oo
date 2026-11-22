using System.Text.RegularExpressions;

namespace Topico12.Geracao;

// Um bloco "class X { ... }" lido de um arquivo PlantUML: o cabeçalho e as linhas de membro.
public record BlocoDeClasse(string Cabecalho, IReadOnlyList<string> Membros);

// Lê de um texto PlantUML só o que o gerador também escreve: blocos de classe e setas de herança.
public static partial class BlocosDoDiagrama
{
    [GeneratedRegex(@"^(abstract class|class|enum) (\w+)(?: <<enumeration>>)? \{[ \t]*\r?\n(.*?)^\}", RegexOptions.Multiline | RegexOptions.Singleline)]
    private static partial Regex Bloco();

    [GeneratedRegex(@"^(\w+) <\|-- (\w+)\s*$", RegexOptions.Multiline)]
    private static partial Regex Heranca();

    // Nome da classe para o bloco lido; o cabeçalho distingue class, abstract class e enum.
    public static Dictionary<string, BlocoDeClasse> LerBlocos(string puml) =>
        Bloco().Matches(puml).ToDictionary(
            m => m.Groups[2].Value,
            m => new BlocoDeClasse(
                m.Groups[1].Value,
                m.Groups[3].Value.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)));

    // Cada herança vira "Base <|-- Filha", indexada pela filha.
    public static Dictionary<string, string> LerHerancas(string puml) =>
        Heranca().Matches(puml).ToDictionary(m => m.Groups[2].Value, m => $"{m.Groups[1].Value} <|-- {m.Groups[2].Value}");
}
