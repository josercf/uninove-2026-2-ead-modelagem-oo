using System.Reflection;

namespace Topico11;

// Quantas classes concretas e quantas abstrações um tipo guarda em campos e recebe no construtor.
public record Dependencias(int Concretas, int Abstracoes);

public static class MedidorDeDependencias
{
    // Só conta tipos do sistema (namespace com o prefixo dado): int, string e a biblioteca base ficam de fora.
    public static Dependencias Medir(Type tipo, string prefixoDoNamespace = "Topico")
    {
        var campos = tipo.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .Select(f => f.FieldType);
        var parametros = tipo.GetConstructors().SelectMany(c => c.GetParameters()).Select(p => p.ParameterType);
        var dependencias = campos.Concat(parametros)
            .Where(t => t.Namespace is not null && t.Namespace.StartsWith(prefixoDoNamespace, StringComparison.Ordinal))
            .Distinct()
            .ToList();
        var abstracoes = dependencias.Count(t => t.IsInterface || (t.IsAbstract && !t.IsSealed));
        return new Dependencias(dependencias.Count - abstracoes, abstracoes);
    }

    // Classes concretas do assembly da interface que a implementam.
    public static IReadOnlyList<Type> Implementacoes(Type interfaceAlvo) =>
        interfaceAlvo.Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && interfaceAlvo.IsAssignableFrom(t)).ToList();
}
