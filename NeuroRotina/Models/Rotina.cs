using SQLite;

namespace NeuroRotina.Models;

public class Rotina
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CriancaId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public DateTime Data { get; set; }

    public bool Concluida { get; set; }
}