using Topico03;
using Topico04;
using Topico06;
using Topico09;
using Topico09.Antes;
using Topico09.Depois;
using BalcaoAntes = Topico09.Antes.Balcao;
using BalcaoDepois = Topico09.Depois.Balcao;

// Mesmo caso de uso (UC01) em dois desenhos. Primeiro o comportamento, depois o acoplamento medido.
var retirada = new DateOnly(2026, 10, 5);

var ua = new UsuarioAnemico("Joana", Perfil.Morador);
var antes = new BalcaoAntes();
Console.WriteLine("Desenho antes:");
Console.WriteLine($"  {antes.Emprestar(ua, new ItemAnemico("Dom Casmurro", TipoDeItem.Livro), retirada).Mensagem}");

var ud = new Usuario("Joana", Perfil.Morador);
var depois = new BalcaoDepois(new RegistroDeEmprestimos());
Console.WriteLine("Desenho depois:");
Console.WriteLine($"  {depois.Emprestar(ud, new Livro("Dom Casmurro", "Machado de Assis"), retirada).Mensagem}");

// Universos: as classes que participam do UC01 em cada desenho.
Type[] universoAntes =
[
    typeof(BalcaoAntes), typeof(UsuarioAnemico), typeof(ItemAnemico), typeof(EmprestimoAnemico),
    typeof(TipoDeItem), typeof(Perfil), typeof(Topico05.ResultadoAtendimento),
];
Type[] universoDepois =
[
    typeof(BalcaoDepois), typeof(RegistroDeEmprestimos), typeof(Usuario), typeof(ItemAcervo), typeof(Livro),
    typeof(Revista), typeof(Midia), typeof(Emprestimo), typeof(Perfil), typeof(Topico05.ResultadoAtendimento),
];

void Relatorio(string nome, Type[] universo)
{
    Console.WriteLine($"\nAcoplamento, desenho {nome}:");
    foreach (var tipo in universo)
    {
        var deps = MedidorDeAcoplamento.Dependencias(tipo, universo);
        Console.WriteLine($"  {tipo.Name,-22} {deps.Count} dependência(s): {string.Join(", ", deps.Select(d => d.Name).Order())}");
    }
    Console.WriteLine($"  Total: {MedidorDeAcoplamento.Total(universo)}");
}

Relatorio("antes", universoAntes);
Relatorio("depois", universoDepois);

Console.WriteLine("\nMembros de outras classes que o balcão usa:");
Console.WriteLine($"  antes : {MedidorDeAcoplamento.MembrosAlheios(typeof(BalcaoAntes), universoAntes).Count}");
Console.WriteLine($"  depois: {MedidorDeAcoplamento.MembrosAlheios(typeof(BalcaoDepois), universoDepois).Count}");
