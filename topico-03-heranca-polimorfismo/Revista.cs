namespace Topico03;

// Revista herda de ItemAcervo e acrescenta o número da edição.
public class Revista(string titulo, int edicao) : ItemAcervo(titulo)
{
    public int Edicao { get; } = edicao;
    public override int PrazoEmDias => 7;
}
