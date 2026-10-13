namespace Topico04;

// Usuário da biblioteca. Implementa RF01, RF02 e RF03 de REQUISITOS.md.
public class Usuario
{
    // Lista privada: só a própria classe acrescenta itens.
    private readonly List<string> _itens = [];

    public Usuario(string nome, Perfil perfil)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome é obrigatório.", nameof(nome));
        Nome = nome;
        Perfil = perfil;
    }

    public string Nome { get; }
    public Perfil Perfil { get; }
    // IReadOnlyList: quem está fora da classe lê os itens, sem Add nem Remove.
    public IReadOnlyList<string> Itens => _itens;
    // private set: a gravação fica restrita aos métodos desta classe.
    public bool TemMultaEmAberto { get; private set; }

    // RF01: o limite de itens simultâneos depende do perfil.
    // switch expression: compara Perfil com cada caso e devolve o valor do primeiro que casar.
    public int Limite => Perfil switch
    {
        Perfil.Morador => 2,
        Perfil.Estudante => 4,
        Perfil.Voluntario => 6,
        // _ casa com qualquer outro valor; aqui, um perfil fora da lista.
        _ => throw new ArgumentOutOfRangeException(nameof(Perfil)),
    };

    public void RegistrarMulta() => TemMultaEmAberto = true;

    // RF03: quitar a multa libera o usuário.
    public void QuitarMulta() => TemMultaEmAberto = false;

    public void Retirar(string titulo)
    {
        // RF02: multa em aberto bloqueia retirada.
        if (TemMultaEmAberto)
            throw new InvalidOperationException("Usuário com multa em aberto não retira item novo.");
        // RF01: recusa a retirada que passaria do limite do perfil.
        if (_itens.Count >= Limite)
            throw new InvalidOperationException($"Usuário atingiu o Limite de {Limite} itens do perfil {Perfil}.");
        _itens.Add(titulo);
    }
}
