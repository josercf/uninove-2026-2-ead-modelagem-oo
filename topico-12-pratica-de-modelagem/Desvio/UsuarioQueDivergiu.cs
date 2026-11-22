using Topico04;

namespace Topico12.Desvio;

// Um Usuario do tópico 04 editado sem atualizar classes.puml, para o teste mostrar o desvio.
// Diferenças: ganhou Suspender(), perdeu QuitarMulta() e Limite passou de int para long.
public class UsuarioQueDivergiu
{
    private readonly List<string> _itens = [];

    public UsuarioQueDivergiu(string nome, Perfil perfil)
    {
        Nome = nome;
        Perfil = perfil;
    }

    public string Nome { get; }
    public Perfil Perfil { get; }
    public IReadOnlyList<string> Itens => _itens;
    public bool TemMultaEmAberto { get; private set; }

    // Desvio 3: o tipo do limite mudou no código.
    public long Limite => Perfil switch
    {
        Perfil.Morador => 2,
        Perfil.Estudante => 4,
        Perfil.Voluntario => 6,
        _ => throw new ArgumentOutOfRangeException(nameof(Perfil)),
    };

    public void RegistrarMulta() => TemMultaEmAberto = true;

    // Desvio 1: método novo que o diagrama não desenha.
    public void Suspender() => RegistrarMulta();

    public void Retirar(string titulo)
    {
        if (TemMultaEmAberto)
            throw new InvalidOperationException("Usuário com multa em aberto não retira item novo.");
        if (_itens.Count >= Limite)
            throw new InvalidOperationException($"Usuário atingiu o Limite de {Limite} itens do perfil {Perfil}.");
        _itens.Add(titulo);
    }
}
