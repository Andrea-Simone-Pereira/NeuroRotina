using SQLite;

namespace NeuroRotina.Models;

/// <summary>
/// Registro de como a criança reagiu à rotina
/// (diagrama de classes: Historico).
/// </summary>
[Table("historico")]
public class Historico
{
    [PrimaryKey, AutoIncrement]
    public int IdHistorico { get; set; }

    [Indexed]
    public int IdCrianca { get; set; }

    public DateTime Data { get; set; } = DateTime.Now;

    [MaxLength(60)]
    public string Comportamento { get; set; } = "Tranquila";

    [MaxLength(300)]
    public string Observacao { get; set; } = string.Empty;

    [MaxLength(60)]
    public string RotinaRelacionada { get; set; } = string.Empty;

    [Ignore]
    public string DataFormatada => Data.ToString("dd/MM/yyyy 'às' HH:mm");

    [Ignore]
    public string Emoji => Comportamento switch
    {
        "Tranquila" => "🙂",
        "Agitada" => "😣",
        "Crise" => "😢",
        "Participativa" => "😄",
        _ => "📝"
    };
}
