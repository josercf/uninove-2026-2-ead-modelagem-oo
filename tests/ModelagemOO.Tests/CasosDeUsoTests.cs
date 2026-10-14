// Cada teste verifica um fluxo do caso de uso UC01 de CASOS_DE_USO.md.
// O nome começa pelo ID do caso de uso e cita o fluxo testado.
using Topico04;
using Topico05;

public class CasosDeUsoTests
{
    [Fact]
    public void UC01_FluxoPrincipal_EmprestimoAceito()
    {
        var balcao = new Balcao();
        var joana = new Usuario("Joana", Perfil.Morador);
        // Passos 2 a 5: o atendente informa usuário e título, e o sistema registra.
        var resultado = balcao.Emprestar(joana, "Dom Casmurro");
        Assert.True(resultado.Aceito);
        Assert.Equal("Empréstimo de \"Dom Casmurro\" registrado para Joana.", resultado.Mensagem);
        // Pós-condição: o título passa a constar nos itens do usuário.
        Assert.Equal(["Dom Casmurro"], joana.Itens);
    }

    [Fact]
    public void UC01_A1_LimiteDoPerfilAtingido_RF01()
    {
        var balcao = new Balcao();
        var joana = new Usuario("Joana", Perfil.Morador);
        balcao.Emprestar(joana, "Dom Casmurro");
        balcao.Emprestar(joana, "Superinteressante");
        // A1: com 2 de 2 itens, o terceiro pedido é recusado (RF01).
        var resultado = balcao.Emprestar(joana, "Central do Brasil");
        Assert.False(resultado.Aceito);
        Assert.Contains("Limite de 2 itens", resultado.Mensagem);
        // RNF01: a recusa começa pela palavra "Usuário".
        Assert.StartsWith("Usuário", resultado.Mensagem);
        // Pós-condição da recusa: a lista de itens não muda.
        Assert.Equal(2, joana.Itens.Count);
    }

    [Fact]
    public void UC01_A2_MultaEmAberto_RF02()
    {
        var balcao = new Balcao();
        var caio = new Usuario("Caio", Perfil.Estudante);
        caio.RegistrarMulta();
        // A2: com multa em aberto, o pedido é recusado mesmo sem nenhum item (RF02).
        var resultado = balcao.Emprestar(caio, "Vidas Secas");
        // Assert.False: o atendimento terminou em recusa.
        Assert.False(resultado.Aceito);
        // A mensagem traz o motivo do RF02 e começa por "Usuário" (RNF01).
        Assert.Contains("multa em aberto", resultado.Mensagem);
        Assert.StartsWith("Usuário", resultado.Mensagem);
        // Assert.Empty: pós-condição da recusa, a lista de itens continua vazia.
        Assert.Empty(caio.Itens);
    }

    [Fact]
    public void UC01_A3_AposQuitarAMulta_RF03()
    {
        var balcao = new Balcao();
        var caio = new Usuario("Caio", Perfil.Estudante);
        caio.RegistrarMulta();
        balcao.Emprestar(caio, "Vidas Secas");
        // A3: o usuário quita a multa e o atendente repete o pedido (RF03).
        caio.QuitarMulta();
        var resultado = balcao.Emprestar(caio, "Vidas Secas");
        Assert.True(resultado.Aceito);
        Assert.Equal(["Vidas Secas"], caio.Itens);
    }
}
