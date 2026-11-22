namespace Topico12.Geracao;

// Um desvio entre o modelo versionado e o gerado do código.
public record Divergencia(string Classe, string Natureza, string Membro = "")
{
    public override string ToString() =>
        Membro == "" ? $"{Classe}: {Natureza}" : $"{Classe}: {Natureza}: {Membro}";
}

public static class ComparadorDeModelo
{
    // "Ausente no código" quer dizer que o diagrama desenha algo que o código não tem.
    // "Ausente no diagrama" quer dizer que o código tem algo que o diagrama não desenha.
    public static List<Divergencia> Comparar(string versionado, string gerado)
    {
        var desenhado = BlocosDoDiagrama.LerBlocos(versionado);
        var emCodigo = BlocosDoDiagrama.LerBlocos(gerado);
        var divergencias = new List<Divergencia>();

        foreach (var nome in desenhado.Keys.Except(emCodigo.Keys))
            divergencias.Add(new(nome, "classe ausente no código"));
        foreach (var nome in emCodigo.Keys.Except(desenhado.Keys))
            divergencias.Add(new(nome, "classe ausente no diagrama"));

        foreach (var nome in desenhado.Keys.Intersect(emCodigo.Keys))  // classes que existem nos dois lados
        {
            var (d, c) = (desenhado[nome], emCodigo[nome]);  // bloco desenhado e bloco gerado da mesma classe
            if (d.Cabecalho != c.Cabecalho)  // "abstract class" contra "class"
                divergencias.Add(new(nome, "cabeçalho diferente", $"diagrama '{d.Cabecalho}', código '{c.Cabecalho}'"));
            foreach (var m in d.Membros.Except(c.Membros))  // Except: membros desenhados que faltam no código
                divergencias.Add(new(nome, "membro ausente no código", m));
            foreach (var m in c.Membros.Except(d.Membros))  // Except: membros do código que faltam no desenho
                divergencias.Add(new(nome, "membro ausente no diagrama", m));
        }

        var herancasDesenhadas = BlocosDoDiagrama.LerHerancas(versionado);
        var herancasDoCodigo = BlocosDoDiagrama.LerHerancas(gerado);
        foreach (var (filha, linha) in herancasDesenhadas.Where(h => !herancasDoCodigo.ContainsValue(h.Value)))
            divergencias.Add(new(filha, "herança ausente no código", linha));
        foreach (var (filha, linha) in herancasDoCodigo.Where(h => !herancasDesenhadas.ContainsValue(h.Value)))
            divergencias.Add(new(filha, "herança ausente no diagrama", linha));
        return divergencias;
    }
}
