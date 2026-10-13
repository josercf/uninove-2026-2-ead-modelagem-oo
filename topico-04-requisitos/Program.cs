using Topico04;

// Demonstra os requisitos RF01 e RF02 com uma moradora.
var joana = new Usuario("Joana", Perfil.Morador);
joana.Retirar("Dom Casmurro");
joana.Retirar("Superinteressante");
Console.WriteLine($"{joana.Nome} ({joana.Perfil}) está com {joana.Itens.Count} de {joana.Limite} itens.");

try { joana.Retirar("Central do Brasil"); }
catch (InvalidOperationException erro) { Console.WriteLine($"RF01: {erro.Message}"); }

var caio = new Usuario("Caio", Perfil.Estudante);
caio.RegistrarMulta();
try { caio.Retirar("Dom Casmurro"); }
catch (InvalidOperationException erro) { Console.WriteLine($"RF02: {erro.Message}"); }
