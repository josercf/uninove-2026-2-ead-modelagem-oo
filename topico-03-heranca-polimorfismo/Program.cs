using Topico03;

// Uma lista do tipo base guarda objetos de três tipos diferentes.
List<ItemAcervo> acervo =
[
    new Livro("Dom Casmurro", "Machado de Assis"),
    new Revista("Superinteressante", 480),
    new Midia("Central do Brasil", "DVD"),
];

var retirada = new DateOnly(2026, 10, 5);

// O mesmo método produz datas diferentes conforme o tipo real do objeto.
foreach (var item in acervo)
{
    var devolucao = item.Emprestar(retirada);
    Console.WriteLine($"{item.GetType().Name,-8} {item.Titulo,-20} prazo {item.PrazoEmDias,2} dias, devolver em {devolucao:dd'/'MM'/'yyyy}");
}
