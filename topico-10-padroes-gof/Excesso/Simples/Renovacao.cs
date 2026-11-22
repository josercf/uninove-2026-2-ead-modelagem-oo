namespace Topico10.Excesso.Simples;

// Versão simples: o limite de renovações é uma constante e a regra cabe numa linha.
public static class Renovacao
{
    public const int Maximo = 1;

    public static bool Permitida(int renovacoesFeitas) => renovacoesFeitas < Maximo;
}
