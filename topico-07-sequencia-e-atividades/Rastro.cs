namespace Topico07;

// Registro das mensagens e das ações de uma execução, na ordem em que aconteceram.
public class Rastro
{
    // Cada mensagem tem o texto exato da linha correspondente em sequencia.puml.
    public List<string> Mensagens { get; } = [];
    // Cada ação tem o texto exato do retângulo correspondente em atividades.puml.
    public List<string> Acoes { get; } = [];

    // Chamada síncrona: seta cheia no diagrama de sequência.
    public void Chamar(string de, string para, string rotulo) => Mensagens.Add($"{de} -> {para} : {rotulo}");

    // Retorno: seta tracejada no diagrama de sequência.
    public void Retornar(string de, string para, string rotulo) => Mensagens.Add($"{de} --> {para} : {rotulo}");

    // Ação do diagrama de atividades.
    public void Acao(string descricao) => Acoes.Add(descricao);
}
