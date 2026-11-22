using Topico04;

namespace Topico09.Antes;

// Desenho "antes": as classes só guardam dados, e o Balcao decide tudo.
public enum TipoDeItem { Livro, Revista, Midia }

// Usuário anêmico: campos públicos de leitura e escrita, sem regra nenhuma.
public class UsuarioAnemico(string nome, Perfil perfil)
{
    public string Nome { get; } = nome;
    public Perfil Perfil { get; } = perfil;
    public List<string> Itens { get; } = [];
    public bool MultaEmAberto { get; set; }
}

// Item anêmico: o prazo não está aqui, está no Balcao.
public class ItemAnemico(string titulo, TipoDeItem tipo)
{
    public string Titulo { get; } = titulo;
    public TipoDeItem Tipo { get; } = tipo;
    public bool Disponivel { get; set; } = true;
}

public class EmprestimoAnemico
{
    public required UsuarioAnemico Usuario { get; init; }
    public required ItemAnemico Item { get; init; }
    public DateOnly Retirada { get; init; }
    public DateOnly Devolucao { get; init; }
}
