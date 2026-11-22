using Topico03;

namespace Topico10.Depois;

// Factory Method: cada fábrica sabe criar um tipo de item e devolve o tipo base.
public abstract class FabricaDeItem
{
    public abstract ItemAcervo Criar(string titulo, string complemento);
}

public class FabricaDeLivro : FabricaDeItem
{
    public override ItemAcervo Criar(string titulo, string complemento) => new Livro(titulo, complemento);
}

public class FabricaDeRevista : FabricaDeItem
{
    public override ItemAcervo Criar(string titulo, string complemento)
    {
        if (!int.TryParse(complemento, out var edicao))
            throw new ArgumentException("A edição da revista deve ser um número inteiro.");
        return new Revista(titulo, edicao);
    }
}

public class FabricaDeMidia : FabricaDeItem
{
    public override ItemAcervo Criar(string titulo, string complemento) => new Midia(titulo, complemento);
}

// O cadastro procura a fábrica pelo tipo e não conhece nenhuma classe concreta de item.
public class CadastroDoAcervo
{
    private readonly Dictionary<string, FabricaDeItem> _fabricas = [];

    public static CadastroDoAcervo ComFabricasPadrao()
    {
        var cadastro = new CadastroDoAcervo();
        cadastro.Registrar("livro", new FabricaDeLivro());
        cadastro.Registrar("revista", new FabricaDeRevista());
        cadastro.Registrar("midia", new FabricaDeMidia());
        return cadastro;
    }

    public void Registrar(string tipo, FabricaDeItem fabrica)
    {
        if (!_fabricas.TryAdd(tipo.ToLowerInvariant(), fabrica))
            throw new InvalidOperationException($"Já existe fábrica para o tipo {tipo}.");
    }

    public ItemAcervo Cadastrar(string tipo, string titulo, string complemento)
    {
        if (!_fabricas.TryGetValue(tipo.ToLowerInvariant(), out var fabrica))
            throw new ArgumentException($"Tipo de item desconhecido: {tipo}.");
        return fabrica.Criar(titulo, complemento);
    }
}
