using SQLite;

namespace NeuroRotina.Models;

/// <summary>
/// Criança acompanhada pelo cuidador (diagrama de classes: Crianca).
/// </summary>
[Table("crianca")]
public class Crianca
{
    [PrimaryKey, AutoIncrement]
    public int IdCrianca { get; set; }

    /// <summary>Chave estrangeira para Usuario.IdUsuario (relação "gerencia").</summary>
    [Indexed]
    public int IdCuidador { get; set; }

    [MaxLength(80), NotNull]
    public string Nome { get; set; } = string.Empty;

    public int Idade { get; set; }

    /// <summary>Nível 1, 2 ou 3 de suporte, conforme DSM-5.</summary>
    [MaxLength(20)]
    public string NivelSuporte { get; set; } = "Nível 1";

    /// <summary>Pictograma do perfil.</summary>
    [MaxLength(8)]
    public string Avatar { get; set; } = "🧒";

    [Ignore]
    public string Resumo => Idade > 0 ? $"{Idade} anos · {NivelSuporte}" : NivelSuporte;
}
