using SQLite;

namespace NeuroRotina.Models;

/// <summary>
/// Sequência de atividades previsível para um momento do dia
/// (diagrama de classes: Rotina).
/// </summary>
[Table("rotina")]
public class Rotina
{
    [PrimaryKey, AutoIncrement]
    public int IdRotina { get; set; }

    [Indexed]
    public int IdCrianca { get; set; }

    [MaxLength(60), NotNull]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Horário de início no formato HH:mm.</summary>
    [MaxLength(5)]
    public string Horario { get; set; } = "07:00";

    [MaxLength(8)]
    public string Icone { get; set; } = "🌅";

    public bool Ativa { get; set; } = true;

    [Ignore]
    public int TotalAtividades { get; set; }

    [Ignore]
    public string Legenda =>
        TotalAtividades == 1 ? "1 atividade" : $"{TotalAtividades} atividades";
}
