// Testes da classe Livro, o primeiro objeto do domínio da Biblioteca Comunitária.
using Topico02;
using Xunit;

public class LivroTests
{
    [Fact]
    public void LivroRecemCriadoEstaDisponivel()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);
        Assert.True(livro.Disponivel);
    }

    [Fact]
    public void EmprestarTornaOLivroIndisponivel()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);
        livro.Emprestar();
        Assert.False(livro.Disponivel);
    }

    [Fact]
    public void EmprestarLivroJaEmprestadoLancaExcecao()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);
        livro.Emprestar();
        Assert.Throws<InvalidOperationException>(() => livro.Emprestar());
    }
}
