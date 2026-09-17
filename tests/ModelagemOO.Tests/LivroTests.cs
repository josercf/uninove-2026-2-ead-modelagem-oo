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
    public void EmprestarLivroJaEmprestadoLancaExcecaoComTituloNaMensagem()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);
        livro.Emprestar();
        var excecao = Assert.Throws<InvalidOperationException>(() => livro.Emprestar());
        Assert.Contains("Dom Casmurro", excecao.Message);
    }

    [Fact]
    public void DevolverTornaOLivroDisponivelDeNovo()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);
        livro.Emprestar();
        livro.Devolver();
        Assert.True(livro.Disponivel);
    }

    [Fact]
    public void DevolverLivroQueNuncaFoiEmprestadoLancaExcecaoComTituloNaMensagem()
    {
        var livro = new Livro("Dom Casmurro", "Machado de Assis", 1899);
        var excecao = Assert.Throws<InvalidOperationException>(() => livro.Devolver());
        Assert.Contains("Dom Casmurro", excecao.Message);
    }
}
