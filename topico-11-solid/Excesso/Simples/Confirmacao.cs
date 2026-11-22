namespace Topico11.Excesso.Simples;

// Versão simples: o texto da confirmação é um só, e cabe numa expressão.
public static class Confirmacao
{
    public static string Emprestimo(string titulo, string nome) => $"Empréstimo de \"{titulo}\" registrado para {nome}.";
}
