namespace Topico03;

// Mídia herda de ItemAcervo e acrescenta o formato (DVD, CD).
public class Midia(string titulo, string formato) : ItemAcervo(titulo)
{
    public string Formato { get; } = formato;
    public override int PrazoEmDias => 3;
}
