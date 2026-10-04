using SQLite;

namespace NeuroRotina.Models;

/// <summary>
/// Classe base de acesso ao sistema (diagrama de classes: Usuario).
/// O campo TipoUsuario diferencia o papel de Cuidador dos demais perfis.
/// </summary>
[Table("usuario")]
public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int IdUsuario { get; set; }

    [MaxLength(80), NotNull]
    public string Nome { get; set; } = string.Empty;

    [Indexed(Name = "idx_email", Unique = true), MaxLength(120), NotNull]
    public string Email { get; set; } = string.Empty;

    /// <summary>Armazenada como hash SHA-256, nunca em texto puro.</summary>
    [NotNull]
    public string Senha { get; set; } = string.Empty;

    [MaxLength(20)]
    public string TipoUsuario { get; set; } = "Cuidador";

    public DateTime CriadoEm { get; set; } = DateTime.Now;

    [Ignore]
    public string Iniciais =>
        string.IsNullOrWhiteSpace(Nome) ? "?" : Nome.Trim()[..1].ToUpperInvariant();
}
