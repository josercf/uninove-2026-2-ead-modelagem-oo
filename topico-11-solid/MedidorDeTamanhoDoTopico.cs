using System.Reflection;
using System.Runtime.CompilerServices;
using Topico10;

namespace Topico11;

// O MedidorDeTamanho do tópico 10 conta tipos no assembly dele. Este conta no assembly deste tópico
// e reaproveita a contagem de linhas, para que os dois tópicos usem a mesma definição de linha com código.
public static class MedidorDeTamanhoDoTopico
{
    public static Tamanho Medir(string espacoDeNomes, string pastaRelativaDaRaiz) =>
        new(ContarTipos(espacoDeNomes), MedidorDeTamanho.ContarLinhasDeCodigo(Path.Combine(MedidorDeTamanho.RaizDoRepositorio(), pastaRelativaDaRaiz)));

    public static int ContarTipos(string espacoDeNomes) =>
        typeof(MedidorDeTamanhoDoTopico).Assembly.GetTypes().Count(t =>
            t.Namespace == espacoDeNomes && !t.IsNested && t.GetCustomAttribute<CompilerGeneratedAttribute>() is null);
}
