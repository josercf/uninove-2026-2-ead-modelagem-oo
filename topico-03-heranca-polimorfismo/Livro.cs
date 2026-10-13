namespace Topico03;

// Livro herda tudo de ItemAcervo e acrescenta o autor.
public class Livro(string titulo, string autor) : ItemAcervo(titulo)
{
    public string Autor { get; } = autor;
    public override int PrazoEmDias => 15;
}
