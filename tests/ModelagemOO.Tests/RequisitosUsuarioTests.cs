// Cada teste verifica um requisito de REQUISITOS.md. O nome começa pelo ID.
using Topico04;

public class RequisitosUsuarioTests
{
    // Theory: o mesmo teste roda uma vez para cada linha InlineData.
    [Theory]
    // Cada InlineData entrega um perfil e o limite esperado para ele.
    [InlineData(Perfil.Morador, 2)]
    [InlineData(Perfil.Estudante, 4)]
    [InlineData(Perfil.Voluntario, 6)]
    // O prefixo RF01 liga o teste à linha RF01 de REQUISITOS.md.
    public void RF01_LimiteDeItensDependeDoPerfil(Perfil perfil, int limite)
    {
        var usuario = new Usuario("Joana", perfil);
        // Retira exatamente o limite do perfil, sem erro.
        for (var i = 0; i < limite; i++) usuario.Retirar($"Item {i}");
        // A retirada seguinte precisa ser recusada.
        var erro = Assert.Throws<InvalidOperationException>(() => usuario.Retirar("Um a mais"));
        Assert.Contains($"Limite de {limite} itens", erro.Message);
    }

    [Fact]
    public void RF02_MultaEmAbertoBloqueiaRetirada()
    {
        var usuario = new Usuario("Joana", Perfil.Voluntario);
        usuario.RegistrarMulta();
        var erro = Assert.Throws<InvalidOperationException>(() => usuario.Retirar("Dom Casmurro"));
        Assert.Contains("multa em aberto", erro.Message);
    }

    [Fact]
    public void RF03_QuitarMultaLiberaNovaRetirada()
    {
        var usuario = new Usuario("Joana", Perfil.Morador);
        usuario.RegistrarMulta();
        usuario.QuitarMulta();
        usuario.Retirar("Dom Casmurro");
        Assert.Equal(["Dom Casmurro"], usuario.Itens);
    }

    [Fact]
    public void RNF01_MensagensDeRecusaEstaoEmPortugues()
    {
        // Recusa por multa (RF02): a mensagem começa por "Usuário".
        var comMulta = new Usuario("Joana", Perfil.Morador);
        comMulta.RegistrarMulta();
        var erroMulta = Assert.Throws<InvalidOperationException>(() => comMulta.Retirar("Dom Casmurro"));
        Assert.StartsWith("Usuário", erroMulta.Message);

        // Recusa por limite (RF01): a mensagem também começa por "Usuário".
        // O laço usa a propriedade Limite, para que este teste dependa só do texto da mensagem.
        var noLimite = new Usuario("Caio", Perfil.Morador);
        for (var i = 0; i < noLimite.Limite; i++) noLimite.Retirar($"Item {i}");
        var erroLimite = Assert.Throws<InvalidOperationException>(() => noLimite.Retirar("Um a mais"));
        Assert.StartsWith("Usuário", erroLimite.Message);
    }

    [Fact]
    public void NomeVazioERecusado()
    {
        Assert.Throws<ArgumentException>(() => new Usuario(" ", Perfil.Morador));
    }
}
