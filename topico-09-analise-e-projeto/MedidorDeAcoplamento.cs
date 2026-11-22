using System.Reflection;
using System.Reflection.Emit;

namespace Topico09;

// Mede o acoplamento entre classes lendo a estrutura (campos, propriedades, métodos) e o corpo
// dos métodos (IL) por reflexão. Duas classes A e B são acopladas quando A menciona B.
public static class MedidorDeAcoplamento
{
    private const BindingFlags Todos =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    // Tabela de instruções IL, indexada pelo valor do código da instrução.
    private static readonly Dictionary<short, OpCode> Instrucoes =
        typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => (OpCode)f.GetValue(null)!)
            .ToDictionary(o => o.Value);

    // Tipos do universo que a classe menciona, fora ela mesma.
    public static IReadOnlySet<Type> Dependencias(Type tipo, IReadOnlyCollection<Type> universo)
    {
        var achados = new HashSet<Type>();
        void Registrar(Type? t) { foreach (var u in Desembrulhar(t)) achados.Add(u); }

        Registrar(tipo.BaseType);
        foreach (var i in tipo.GetInterfaces()) Registrar(i);
        foreach (var c in tipo.GetFields(Todos)) Registrar(c.FieldType);
        foreach (var p in tipo.GetProperties(Todos)) Registrar(p.PropertyType);
        foreach (var m in MetodosDe(tipo))
        {
            if (m is MethodInfo mi) Registrar(mi.ReturnType);
            foreach (var p in m.GetParameters()) Registrar(p.ParameterType);
            var corpo = m.GetMethodBody();
            if (corpo is null) continue;
            foreach (var v in corpo.LocalVariables) Registrar(v.LocalType);
            foreach (var membro in MembrosDoCorpo(m))
            {
                Registrar(membro.DeclaringType);
                if (membro is FieldInfo f) Registrar(f.FieldType);
            }
        }
        return achados.Where(t => t != tipo && universo.Contains(t)).ToHashSet();
    }

    // Membros de outras classes do universo que o corpo dos métodos usa: chamadas, campos e construtores.
    public static IReadOnlySet<MemberInfo> MembrosAlheios(Type tipo, IReadOnlyCollection<Type> universo)
    {
        var achados = new HashSet<MemberInfo>();
        foreach (var m in MetodosDe(tipo))
            foreach (var membro in MembrosDoCorpo(m))
                if (membro.DeclaringType is { } dono && dono != tipo && universo.Contains(dono))
                    achados.Add(membro);
        return achados;
    }

    // Classes do universo cujo corpo de método executa new sobre o tipo alvo.
    public static IReadOnlyList<Type> Criadores(Type alvo, IReadOnlyCollection<Type> universo) =>
        universo.Where(t => t != alvo
                && MetodosDe(t).Any(m => MembrosDoCorpo(m).OfType<ConstructorInfo>().Any(c => c.DeclaringType == alvo)))
            .ToList();

    // Soma das dependências de todas as classes do universo.
    public static int Total(IReadOnlyCollection<Type> universo) =>
        universo.Sum(t => Dependencias(t, universo).Count);

    private static IEnumerable<MethodBase> MetodosDe(Type tipo) =>
        tipo.GetMethods(Todos).Cast<MethodBase>().Concat(tipo.GetConstructors(Todos));

    // IReadOnlyList<Usuario> devolve IReadOnlyList e Usuario; Usuario[] devolve Usuario.
    private static IEnumerable<Type> Desembrulhar(Type? t)
    {
        if (t is null) yield break;
        if (t.HasElementType) { foreach (var e in Desembrulhar(t.GetElementType())) yield return e; yield break; }
        yield return t.IsGenericType ? t.GetGenericTypeDefinition() : t;
        if (t.IsGenericType)
            foreach (var a in t.GetGenericArguments())
                foreach (var e in Desembrulhar(a)) yield return e;
    }

    // Lê os bytes IL do método e resolve cada operando que aponta para um membro ou tipo.
    private static IEnumerable<MemberInfo> MembrosDoCorpo(MethodBase metodo)
    {
        var il = metodo.GetMethodBody()?.GetILAsByteArray();
        if (il is null) yield break;
        var modulo = metodo.Module;
        var tiposGenericos = metodo.DeclaringType!.IsGenericType ? metodo.DeclaringType.GetGenericArguments() : null;
        var metodoGenerico = metodo.IsGenericMethod ? metodo.GetGenericArguments() : null;

        for (var i = 0; i < il.Length;)
        {
            // Instrução de 1 byte, ou de 2 quando o primeiro byte é 0xFE.
            var codigo = il[i] == 0xFE ? (short)(0xFE00 | il[i + 1]) : il[i];
            i += il[i] == 0xFE ? 2 : 1;
            var op = Instrucoes[codigo];
            switch (op.OperandType)
            {
                case OperandType.InlineMethod:
                case OperandType.InlineField:
                case OperandType.InlineType:
                case OperandType.InlineTok:
                    MemberInfo? membro = null;
                    try { membro = modulo.ResolveMember(BitConverter.ToInt32(il, i), tiposGenericos, metodoGenerico); }
                    catch (ArgumentException) { }
                    if (membro is not null) yield return membro;
                    i += 4;
                    break;
                case OperandType.InlineNone: break;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar: i += 1; break;
                case OperandType.InlineVar: i += 2; break;
                case OperandType.InlineI8:
                case OperandType.InlineR: i += 8; break;
                case OperandType.InlineSwitch: i += 4 + 4 * BitConverter.ToInt32(il, i); break;
                default: i += 4; break;
            }
        }
    }
}
