using SQLite;

namespace NeuroRotina.Models;

/// <summary>
/// Aviso antecipado de transição, gerado por uma rotina
/// (diagrama de classes: Alerta).
/// </summary>
[Table("alerta")]
public class Alerta
{
    [PrimaryKey, AutoIncrement]
    public int IdAlerta { get; set; }

    [Indexed]
    public int IdRotina { get; set; }

    /// <summary>Horário de disparo no formato HH:mm.</summary>
    [MaxLength(5)]
    public string Horario { get; set; } = "06:50";

    [MaxLength(140)]
    public string Mensagem { get; set; } = string.Empty;

    /// <summary>Minutos de antecedência usados para gerar o horário.</summary>
    public int AntecedenciaMinutos { get; set; } = 10;

    public bool Ativo { get; set; } = true;

    /// <summary>Evita repetir o mesmo alerta várias vezes no mesmo dia.</summary>
    public DateTime? UltimoDisparo { get; set; }
}
