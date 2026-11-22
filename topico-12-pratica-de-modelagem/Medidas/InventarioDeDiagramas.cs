using System.Text;
using System.Text.RegularExpressions;

namespace Topico12.Medidas;

public record ArquivoDeModelo(string Nome, string Topico, int Linhas, int Bytes, string Texto);

// Os arquivos .puml dos tópicos 05 a 08: o modelo da disciplina guardado como texto.
public class InventarioDeDiagramas
{
    private static readonly string[] Pastas =
    [
        "topico-05-casos-de-uso", "topico-06-classes-e-objetos",
        "topico-07-sequencia-e-atividades", "topico-08-estados-componentes-implantacao",
    ];

    public IReadOnlyList<ArquivoDeModelo> Arquivos { get; }

    private InventarioDeDiagramas(IReadOnlyList<ArquivoDeModelo> arquivos) => Arquivos = arquivos;

    // Lê só os .puml no primeiro nível de cada pasta, o que deixa de fora Requisito/Depois.
    public static InventarioDeDiagramas Ler(string raiz) =>
        new(Pastas
            .SelectMany(p => Directory.GetFiles(Path.Combine(raiz, p), "*.puml"))
            .OrderBy(f => f, StringComparer.Ordinal)
            .Select(f =>
            {
                var texto = File.ReadAllText(f);
                return new ArquivoDeModelo(Path.GetFileName(f), Path.GetFileName(Path.GetDirectoryName(f))!,
                    texto.Split('\n').Count(l => l.Trim() != ""), Encoding.UTF8.GetByteCount(texto), texto);
            })
            .ToList());

    // Quantas linhas citam o nome e em quantos arquivos: o trabalho de um renome feito à mão em texto.
    public (int Linhas, int Arquivos) OcorrenciasDe(string nome)
    {
        var padrao = new Regex($@"\b{Regex.Escape(nome)}\b");
        var porArquivo = Arquivos.Select(a => a.Texto.Split('\n').Count(padrao.IsMatch)).ToList();
        return (porArquivo.Sum(), porArquivo.Count(n => n > 0));
    }
}
