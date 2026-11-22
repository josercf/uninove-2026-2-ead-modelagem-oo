namespace Topico12.Geracao;

// Quanto do modelo versionado o gerador recupera do código.
public record CoberturaDaEngenhariaReversa(int LinhasDeModelo, int LinhasGeradas, IReadOnlyList<string> LinhasNaoRecuperaveis)
{
    // Linha de modelo é a que desenha algo: fora comentário, skinparam, hide e @startuml/@enduml.
    public static CoberturaDaEngenhariaReversa Medir(string versionado, string gerado)
    {
        var geradas = gerado.Split('\n').Select(l => l.Trim()).Where(l => l != "").ToList();
        var modelo = versionado.Split('\n').Select(l => l.Trim()).Where(EhLinhaDeModelo).ToList();
        var naoRecuperaveis = new List<string>();
        // Cada linha gerada casa uma única vez: as chaves "}" se repetem, uma por bloco.
        foreach (var linha in modelo)
        {
            if (!geradas.Remove(linha))
                naoRecuperaveis.Add(linha);
        }
        return new(modelo.Count, modelo.Count - naoRecuperaveis.Count, naoRecuperaveis);
    }

    private static bool EhLinhaDeModelo(string linha) =>
        linha != "" && !linha.StartsWith('\'') && !linha.StartsWith("skinparam")
        && !linha.StartsWith("hide ") && !linha.StartsWith("@start") && !linha.StartsWith("@end");
}
