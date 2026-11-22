namespace Topico11.Responsabilidade.Depois;

// Única razão de mudança: onde e como as linhas do rastro ficam guardadas.
public class RastroDoBalcao
{
    private readonly List<string> _linhas = [];

    public IReadOnlyList<string> Linhas => _linhas;

    [MotivoDeMudanca("registro do rastro")]
    public void Anotar(string linha) => _linhas.Add(linha);
}
