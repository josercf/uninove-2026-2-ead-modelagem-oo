using Topico04;
using Topico05;

// Encena o caso de uso UC01 no balcão: fluxo principal e fluxos A1, A2 e A3.
var balcao = new Balcao();

// Função local: atende um pedido e imprime o fluxo, o resultado e a mensagem.
void Atender(string fluxo, Usuario usuario, string titulo)
{
    var resultado = balcao.Emprestar(usuario, titulo);
    // Operador ?: escolhe "aceito" quando Aceito é true e "recusado" quando é false.
    var situacao = resultado.Aceito ? "aceito" : "recusado";
    Console.WriteLine($"{fluxo}: {usuario.Nome} ({usuario.Perfil}) pede \"{titulo}\", {situacao}.");
    Console.WriteLine($"    {resultado.Mensagem}");
}

var joana = new Usuario("Joana", Perfil.Morador);
Atender("Fluxo principal", joana, "Dom Casmurro");
Atender("Fluxo principal", joana, "Superinteressante");
Atender("A1, RF01", joana, "Central do Brasil");

var caio = new Usuario("Caio", Perfil.Estudante);
caio.RegistrarMulta();
Atender("A2, RF02", caio, "Vidas Secas");
caio.QuitarMulta();
Atender("A3, RF03", caio, "Vidas Secas");
