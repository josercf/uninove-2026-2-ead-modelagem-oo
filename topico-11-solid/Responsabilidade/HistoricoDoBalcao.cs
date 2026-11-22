using Topico11.Responsabilidade.Antes;

namespace Topico11.Responsabilidade;

// Uma versão do Balcão e os motivos de mudança que o código dela reúne.
public record VersaoDoBalcao(string Versao, IReadOnlyList<string> Motivos);

// O git guarda um commit por arquivo do Balcão (tópicos 05, 07 e 09), então não há histórico de linhas para medir.
// A medida é por versão de pasta. As três primeiras linhas são classificação feita à mão sobre o código do tópico
// (critério: o corpo decide uma regra, monta um texto ou grava um registro que outra pessoa ou outro documento mudaria).
// A última linha é medida por reflexão sobre BalcaoDeTudo.
public static class HistoricoDoBalcao
{
    public static IReadOnlyList<VersaoDoBalcao> Versoes { get; } =
    [
        new("topico-05", ["regra de empréstimo", "texto das mensagens"]),
        new("topico-07", ["regra de empréstimo", "regra de multa", "texto das mensagens", "registro do rastro"]),
        new("topico-09 depois", ["ordem do atendimento", "texto das mensagens"]),
        new("topico-11 antes", MedidorDeMotivos.Motivos(typeof(BalcaoDeTudo)).ToList()),
    ];
}
