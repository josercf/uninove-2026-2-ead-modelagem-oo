using System.Reflection;
using System.Runtime.CompilerServices;

namespace Topico12.Geracao;

// Engenharia reversa: lê tipos C# por reflexão e escreve os blocos de classe em PlantUML.
// A convenção é a de classes.puml (tópico 06): campos, propriedades, construtores não públicos
// e métodos, nesta ordem, cada grupo na ordem de declaração do código.
public static class GeradorDeDiagramaDeClasses
{
    // Só os membros declarados no próprio tipo entram; o herdado aparece pela seta de herança.
    private const BindingFlags Declarados =
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    private static readonly Dictionary<Type, string> Apelidos = new()
    {
        [typeof(string)] = "string", [typeof(bool)] = "bool", [typeof(int)] = "int", [typeof(long)] = "long",
        [typeof(decimal)] = "decimal", [typeof(double)] = "double", [typeof(void)] = "void",
    };

    // Gera os blocos de todos os tipos e, depois, uma linha de herança por subclasse do conjunto.
    public static string Gerar(IEnumerable<Type> tipos)
    {
        var lista = tipos.ToList();
        var modelo = lista.ToHashSet();
        var blocos = lista.Select(t => GerarBloco(t, modelo));
        // Exemplo de linha: "ItemAcervo <|-- Livro".
        var herancas = lista
            .Where(t => t.BaseType is not null && modelo.Contains(t.BaseType))
            .Select(t => $"{t.BaseType!.Name} <|-- {t.Name}");
        return string.Join("\n\n", blocos) + "\n\n" + string.Join('\n', herancas) + "\n";
    }

    // Arquivo PlantUML completo: o estilo do repositório mais os blocos gerados. O arquivo
    // é regenerado por "dotnet run --project topico-12-pratica-de-modelagem -- --gravar".
    public static string GerarArquivo(IEnumerable<Type> tipos) =>
        string.Join('\n',
            "@startuml",
            "' Gerado por reflexão a partir do código C#, tópico 12. Não editar à mão.",
            "' O teste ModeloComoCodigoTests confere este arquivo contra o código a cada execução.",
            "skinparam backgroundColor white",
            "skinparam dpi 150",
            "skinparam defaultFontName Arial",
            "skinparam defaultFontSize 14",
            "skinparam shadowing false",
            "skinparam classAttributeIconSize 0",
            "skinparam ArrowColor #1F3A5F",
            "skinparam ClassBorderColor #1F3A5F",
            "skinparam ClassBackgroundColor #E8F0FA",
            "skinparam ClassFontColor #000000",
            "skinparam ClassAttributeFontColor #000000",
            "hide circle",
            "hide empty members",
            "",
            Gerar(tipos).TrimEnd(),
            "@enduml") + "\n";

    // Gera o bloco de um tipo. "modelo" diz quais classes o diagrama desenha: uma propriedade
    // cujo tipo é uma delas vira associação (seta), não linha de membro. O nome opcional
    // permite desenhar um tipo do código sob o nome de outro, para comparar versões.
    public static string GerarBloco(Type tipo, IReadOnlySet<Type> modelo, string? nomeNoDiagrama = null)
    {
        var nome = nomeNoDiagrama ?? tipo.Name;
        if (tipo.IsEnum)
            return $"enum {nome} <<enumeration>> {{\n" + string.Concat(Enum.GetNames(tipo).Select(v => $"  {v}\n")) + "}";

        var linhas = new List<string>();
        // Campos: os gerados pelo compilador (propriedade automática) ficam de fora.
        foreach (var c in tipo.GetFields(Declarados).Where(c => !c.IsDefined(typeof(CompilerGeneratedAttribute))).OrderBy(c => c.MetadataToken))
            linhas.Add($"{Visibilidade(c.IsPublic, c.IsFamily || c.IsFamilyOrAssembly)}{c.Name} : {Nome(c.FieldType)}");
        // Propriedades: associação com classe do modelo sai da caixa.
        foreach (var p in tipo.GetProperties(Declarados).OrderBy(p => p.MetadataToken))
        {
            if (EhAssociacao(p.PropertyType, modelo)) continue;
            var acesso = p.GetMethod ?? p.SetMethod!;
            linhas.Add($"{Visibilidade(acesso.IsPublic, acesso.IsFamily || acesso.IsFamilyOrAssembly)}{Abstrato(acesso)}{p.Name} : {Nome(p.PropertyType)}");
        }
        // Construtores: o público é omitido por convenção; só o protegido ou privado aparece.
        foreach (var k in tipo.GetConstructors(Declarados).Where(k => !k.IsPublic).OrderBy(k => k.MetadataToken))
            linhas.Add($"{Visibilidade(false, k.IsFamily || k.IsFamilyOrAssembly)}{nome}({Parametros(k)})");
        // Métodos: os acessores de propriedade (get_/set_) são "especiais" e ficam de fora.
        foreach (var m in tipo.GetMethods(Declarados).Where(m => !m.IsSpecialName && !m.IsDefined(typeof(CompilerGeneratedAttribute))).OrderBy(m => m.MetadataToken))
            linhas.Add($"{Visibilidade(m.IsPublic, m.IsFamily || m.IsFamilyOrAssembly)}{Abstrato(m)}{m.Name}({Parametros(m)}) : {Nome(m.ReturnType)}");

        var cabecalho = tipo.IsAbstract ? "abstract class" : "class";
        return $"{cabecalho} {nome} {{\n" + string.Concat(linhas.Select(l => $"  {l}\n")) + "}";
    }

    // Classe do modelo, sozinha ou como argumento de tipo (List<Reserva>), é associação; enum não é.
    private static bool EhAssociacao(Type tipo, IReadOnlySet<Type> modelo) =>
        (modelo.Contains(tipo) && !tipo.IsEnum)
        || (tipo.IsGenericType && tipo.GetGenericArguments().Any(a => modelo.Contains(a) && !a.IsEnum));

    private static string Visibilidade(bool publico, bool protegido) => publico ? "+" : protegido ? "#" : "-";

    private static string Abstrato(MethodBase metodo) => metodo.IsAbstract ? "{abstract} " : "";

    private static string Parametros(MethodBase metodo) =>
        string.Join(", ", metodo.GetParameters().Select(p => $"{p.Name} : {Nome(p.ParameterType)}"));

    // Escreve o tipo como no C#: List`1 de String vira List<string>.
    private static string Nome(Type tipo) =>
        Apelidos.TryGetValue(tipo, out var apelido) ? apelido
        : tipo.IsGenericType ? $"{tipo.Name[..tipo.Name.IndexOf('`')]}<{string.Join(", ", tipo.GetGenericArguments().Select(Nome))}>"
        : tipo.Name;
}
