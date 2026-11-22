using System.Reflection;
using System.Runtime.CompilerServices;
using Topico04;
using Topico10;

namespace Topico11.Refatoracao;

// Quantos métodos e quantas linhas com código um passo tem.
public record MedidaDoPasso(int Metodos, int Linhas);

// Registro dos cinco passos: cada um é uma versão do mesmo método Resumir, com o mesmo contrato.
public static class Passos
{
    private static readonly (string Nome, Func<Perfil, DateOnly, DateOnly, string, string> Resumir)[] Versoes =
    [
        (nameof(Passo0), Passo0.Resumir),
        (nameof(Passo1), Passo1.Resumir),
        (nameof(Passo2), Passo2.Resumir),
        (nameof(Passo3), Passo3.Resumir),
        (nameof(Passo4), Passo4.Resumir),
    ];

    public static IReadOnlyList<string> Nomes => Versoes.Select(v => v.Nome).ToList();

    public static string Resumir(string passo, Perfil perfil, DateOnly prevista, DateOnly entrega, string titulo)
    {
        var versao = Versoes.FirstOrDefault(v => v.Nome == passo);
        if (versao.Resumir is null)
            throw new ArgumentException($"Passo desconhecido: {passo}.", nameof(passo));
        return versao.Resumir(perfil, prevista, entrega, titulo);
    }

    public static MedidaDoPasso Medir(string passo)
    {
        var tipo = typeof(Passos).Assembly.GetType($"Topico11.Refatoracao.{passo}") ?? throw new ArgumentException(passo, nameof(passo));
        var metodos = tipo.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Count(m => !m.IsSpecialName && m.GetCustomAttribute<CompilerGeneratedAttribute>() is null);
        return new MedidaDoPasso(metodos, MedidorDeTamanho.ContarLinhasDeArquivo(ArquivoDoPasso(passo)));
    }

    public static int LinhasDoMetodoPrincipal(string passo) => LinhasDoMetodoNoArquivo(ArquivoDoPasso(passo), "Resumir");

    private static string ArquivoDoPasso(string passo) =>
        Path.Combine(MedidorDeTamanho.RaizDoRepositorio(), "topico-11-solid", "Refatoracao", $"{passo}.cs");

    // Conta as linhas com código do método de bloco chamado nome, da assinatura até a chave que fecha.
    public static int LinhasDoMetodoNoArquivo(string arquivo, string nome)
    {
        var linhas = File.ReadAllLines(arquivo).Select(l => l.Trim()).ToList();
        var inicio = linhas.FindIndex(l => l.Contains($" {nome}("));
        if (inicio < 0)
            throw new InvalidOperationException($"Método {nome} não encontrado em {arquivo}.");
        var total = 0;
        var profundidade = 0;
        var abriu = false;
        for (var i = inicio; i < linhas.Count; i++)
        {
            var l = linhas[i];
            if (l.Length > 0 && !l.StartsWith("//") && l != "{" && l != "}")
                total++;
            profundidade += l.Count(c => c == '{') - l.Count(c => c == '}');
            abriu |= l.Contains('{');
            if (abriu && profundidade == 0)
                break;
        }
        return total;
    }
}
