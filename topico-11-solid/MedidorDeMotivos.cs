using System.Reflection;
using System.Runtime.CompilerServices;

namespace Topico11;

// Conta os motivos de mudança de um tipo pelos rótulos dos seus métodos.
public static class MedidorDeMotivos
{
    private const BindingFlags Todos =
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    // Métodos escritos à mão: sem os de propriedade e sem os gerados pelo compilador.
    private static IEnumerable<MethodInfo> Metodos(Type tipo) =>
        tipo.GetMethods(Todos).Where(m => !m.IsSpecialName && m.GetCustomAttribute<CompilerGeneratedAttribute>() is null);

    public static IReadOnlyCollection<string> Motivos(Type tipo) =>
        Metodos(tipo).SelectMany(m => m.GetCustomAttributes<MotivoDeMudancaAttribute>()).Select(a => a.Motivo).Distinct().ToList();

    public static int Contar(Type tipo) => Motivos(tipo).Count;

    // Método sem rótulo escaparia da contagem, e o teste exige que a lista seja vazia.
    public static IEnumerable<string> MetodosSemMotivo(Type tipo) =>
        Metodos(tipo).Where(m => !m.GetCustomAttributes<MotivoDeMudancaAttribute>().Any()).Select(m => m.Name);

    public static IEnumerable<Type> TiposDoNamespace(string espacoDeNomes) =>
        typeof(MedidorDeMotivos).Assembly.GetTypes().Where(t =>
            t.Namespace == espacoDeNomes && !t.IsNested && t.GetCustomAttribute<CompilerGeneratedAttribute>() is null);
}
