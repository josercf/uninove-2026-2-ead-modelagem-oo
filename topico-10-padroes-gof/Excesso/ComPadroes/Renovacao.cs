namespace Topico10.Excesso.ComPadroes;

// Strategy para uma regra que tem uma única variante e nenhuma previsão de outra.
public interface IPoliticaDeRenovacao
{
    bool Permitida(int renovacoesFeitas);
}

public class RenovacaoUnica : IPoliticaDeRenovacao
{
    public bool Permitida(int renovacoesFeitas) => renovacoesFeitas < 1;
}

// Factory que só sabe devolver a única política existente.
public static class FabricaDePoliticaDeRenovacao
{
    public static IPoliticaDeRenovacao Criar() => new RenovacaoUnica();
}

// Observer para um aviso que ninguém pediu.
public interface IObservadorDeRenovacao
{
    void Negada(int renovacoesFeitas);
}

public class RegistroDeRenovacoesNegadas : IObservadorDeRenovacao
{
    public List<int> RenovacoesNegadas { get; } = [];

    public void Negada(int renovacoesFeitas) => RenovacoesNegadas.Add(renovacoesFeitas);
}

public class ControleDeRenovacao(IPoliticaDeRenovacao politica)
{
    private readonly List<IObservadorDeRenovacao> _observadores = [];

    public void Inscrever(IObservadorDeRenovacao observador) => _observadores.Add(observador);

    public bool Tentar(int renovacoesFeitas)
    {
        var permitida = politica.Permitida(renovacoesFeitas);
        if (!permitida)
            foreach (var observador in _observadores)
                observador.Negada(renovacoesFeitas);
        return permitida;
    }
}
