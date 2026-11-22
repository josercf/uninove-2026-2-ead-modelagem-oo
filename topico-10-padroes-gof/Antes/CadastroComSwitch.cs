using Topico03;

namespace Topico10.Antes;

// Desenho antes: o cadastro escolhe a classe concreta com um switch sobre o tipo.
public static class CadastroComSwitch
{
    public static ItemAcervo Cadastrar(string tipo, string titulo, string complemento)
    {
        return tipo.ToLowerInvariant() switch
        {
            "livro" => new Livro(titulo, complemento),
            "revista" => new Revista(titulo, LerEdicao(complemento)),
            "midia" => new Midia(titulo, complemento),
            // Tipo novo obriga a editar este método e todo outro switch igual a ele.
            _ => throw new ArgumentException($"Tipo de item desconhecido: {tipo}."),
        };
    }

    private static int LerEdicao(string texto)
    {
        if (!int.TryParse(texto, out var edicao))
            throw new ArgumentException("A edição da revista deve ser um número inteiro.");
        return edicao;
    }
}
