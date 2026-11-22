using System.Globalization;
using Topico03;
using Topico04;
using Topico07;

namespace Topico11.Responsabilidade.Antes;

// Desenho antes: o balcão que os tópicos 05, 07 e 10 pedem, com tudo no mesmo tipo.
// Cada rótulo [MotivoDeMudanca] diz por que alguém abriria este método para editar.
public class BalcaoDeTudo
{
    private static readonly CultureInfo Pt = CultureInfo.GetCultureInfo("pt-BR");
    private readonly List<string> _rastro = [];

    public IReadOnlyList<string> Rastro => _rastro;

    [MotivoDeMudanca("regra de empréstimo")]
    [MotivoDeMudanca("texto das mensagens")]
    [MotivoDeMudanca("registro do rastro")]
    public string Emprestar(Usuario usuario, string titulo)
    {
        try
        {
            // RF01 e RF02: quem decide é o Usuario, e o balcão só repassa a recusa.
            usuario.Retirar(titulo);
        }
        catch (InvalidOperationException recusa)
        {
            Anotar($"Empréstimo de {titulo} para {usuario.Nome}: recusado");
            return recusa.Message;
        }
        Anotar($"Empréstimo de {titulo} para {usuario.Nome}: aceito");
        return $"Empréstimo de \"{titulo}\" registrado para {usuario.Nome}.";
    }

    [MotivoDeMudanca("regra de multa")]
    [MotivoDeMudanca("texto das mensagens")]
    [MotivoDeMudanca("registro do rastro")]
    public ComprovanteDeDevolucao Devolver(Usuario usuario, ItemAcervo item, DateOnly prevista, DateOnly entrega)
    {
        item.Devolver();
        var dias = Math.Max(0, entrega.DayNumber - prevista.DayNumber);
        var multa = dias * ValorPorDia(usuario.Perfil);
        if (dias > 0)
            usuario.RegistrarMulta();
        Anotar($"Devolução de {item.Titulo}: {dias} dia(s), multa R$ {multa.ToString("0.00", Pt)}");
        return new ComprovanteDeDevolucao(item.Titulo, dias, multa);
    }

    // Valores de exemplo do tópico 10: multa por dia de atraso, em reais.
    [MotivoDeMudanca("regra de multa")]
    private static decimal ValorPorDia(Perfil perfil) => perfil switch
    {
        Perfil.Morador => 1.00m,
        Perfil.Estudante => 0.50m,
        Perfil.Voluntario => 0.25m,
        _ => throw new ArgumentOutOfRangeException(nameof(perfil)),
    };

    [MotivoDeMudanca("registro do rastro")]
    private void Anotar(string linha) => _rastro.Add(linha);
}
