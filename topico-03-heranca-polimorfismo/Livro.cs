namespace Topico03;

// Livro herda tudo de ItemAcervo e acrescenta o autor.
// Os parênteses depois de Livro formam o construtor primário.
// ": ItemAcervo(titulo)" declara a herança e repassa o título à base.
public class Livro(string titulo, string autor) : ItemAcervo(titulo)
{
    // Autor é o único dado que o livro acrescenta à classe base.
    public string Autor { get; } = autor;
    // override cumpre a obrigação do prazo abstrato: 15 dias.
    public override int PrazoEmDias => 15;
}
