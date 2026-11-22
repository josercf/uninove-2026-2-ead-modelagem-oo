using System.Reflection;
using System.Runtime.CompilerServices;

namespace Topico10;

// Tamanho de um conjunto de código: tipos (classes e interfaces) e linhas com código.
public record Tamanho(int Tipos, int Linhas);

// Mede o tamanho de uma versão pela reflexão (tipos de um namespace) e pelo texto dos arquivos (linhas).
public static class MedidorDeTamanho
{
    public static Tamanho Medir(string espacoDeNomes, string pastaRelativaDaRaiz) =>
        new(ContarTipos(espacoDeNomes), ContarLinhasDeCodigo(Path.Combine(RaizDoRepositorio(), pastaRelativaDaRaiz)));

    // Conta classes, records e interfaces do namespace exato, sem tipos gerados pelo compilador.
    public static int ContarTipos(string espacoDeNomes) =>
        Assembly.GetExecutingAssembly().GetTypes().Count(t =>
            t.Namespace == espacoDeNomes
            && !t.IsNested
            && t.GetCustomAttribute<CompilerGeneratedAttribute>() is null);

    // Linha com código: não vazia, sem comentário de linha, sem chave isolada, sem using e sem namespace.
    public static int ContarLinhasDeCodigo(string pasta) =>
        Directory.GetFiles(pasta, "*.cs", SearchOption.AllDirectories).Sum(ContarLinhasDeArquivo);

    public static int ContarLinhasDeArquivo(string arquivo) =>
        File.ReadAllLines(arquivo) // uma string para cada linha do arquivo
            .Select(l => l.Trim()) // tira os espaços das pontas
            .Count(l => l.Length > 0 // conta só as linhas não vazias
                && !l.StartsWith("//") // que não começam com comentário
                && l != "{" && l != "}" // que não são chave isolada
                && !l.StartsWith("using ") // que não são using
                && !l.StartsWith("namespace ")); // e que não são namespace

    // Sobe a partir da pasta do executável até achar a solução.
    public static string RaizDoRepositorio()
    {
        var pasta = new DirectoryInfo(AppContext.BaseDirectory);
        while (pasta is not null && !File.Exists(Path.Combine(pasta.FullName, "ModelagemOO.sln")))
            pasta = pasta.Parent;
        return pasta?.FullName ?? throw new DirectoryNotFoundException("ModelagemOO.sln não encontrada acima do executável.");
    }
}
