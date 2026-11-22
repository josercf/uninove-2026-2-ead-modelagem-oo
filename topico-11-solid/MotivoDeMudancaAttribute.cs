namespace Topico11;

// Rótulo de um motivo de mudança: a razão pela qual alguém editaria o método.
// AllowMultiple: um método que mistura duas razões recebe dois rótulos.
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class MotivoDeMudancaAttribute(string motivo) : Attribute
{
    public string Motivo { get; } = motivo;
}
