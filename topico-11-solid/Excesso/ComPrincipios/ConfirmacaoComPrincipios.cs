namespace Topico11.Excesso.ComPrincipios;

// Inversão de dependência e segregação de interfaces: o texto e a entrega ficam atrás de duas interfaces.
public interface IFormatadorDeConfirmacao
{
    string Formatar(string titulo, string nome);
}

public interface IEntregadorDeConfirmacao
{
    void Entregar(string texto);
}

// Aberto-fechado: outro idioma entraria como outra implementação, sem editar o serviço.
public class FormatadorEmPortugues : IFormatadorDeConfirmacao
{
    public string Formatar(string titulo, string nome) => $"Empréstimo de \"{titulo}\" registrado para {nome}.";
}

public class EntregadorEmMemoria : IEntregadorDeConfirmacao
{
    public List<string> Entregues { get; } = [];

    public void Entregar(string texto) => Entregues.Add(texto);
}

// Responsabilidade única: o serviço só liga o formatador ao entregador.
public class ServicoDeConfirmacao(IFormatadorDeConfirmacao formatador, IEntregadorDeConfirmacao entregador)
{
    public string Confirmar(string titulo, string nome)
    {
        var texto = formatador.Formatar(titulo, nome);
        entregador.Entregar(texto);
        return texto;
    }
}

public static class FabricaDeServicoDeConfirmacao
{
    public static ServicoDeConfirmacao Criar(IEntregadorDeConfirmacao entregador) => new(new FormatadorEmPortugues(), entregador);
}
