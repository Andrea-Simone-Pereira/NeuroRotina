using SQLite;

namespace NeuroRotina.Models;

/// <summary>
/// Passo individual de uma rotina, com apoio visual e sonoro
/// (diagrama de classes: Atividade).
/// </summary>
[Table("atividade")]
public class Atividade
{
    [PrimaryKey, AutoIncrement]
    public int IdAtividade { get; set; }

    [Indexed]
    public int IdRotina { get; set; }

    [MaxLength(60), NotNull]
    public string NomeAtividade { get; set; } = string.Empty;

    /// <summary>Pictograma exibido quando não há foto escolhida.</summary>
    [MaxLength(8)]
    public string Icone { get; set; } = "⭐";

    /// <summary>Caminho da imagem escolhida na galeria (opcional).</summary>
    public string Imagem { get; set; } = string.Empty;

    /// <summary>Texto falado em voz alta pela síntese de voz.</summary>
    [MaxLength(120)]
    public string Audio { get; set; } = string.Empty;

    public int Ordem { get; set; }

    public bool Concluida { get; set; }

    [Ignore]
    public bool TemImagem => !string.IsNullOrWhiteSpace(Imagem) && File.Exists(Imagem);

    [Ignore]
    public string TextoFalado =>
        string.IsNullOrWhiteSpace(Audio) ? NomeAtividade : Audio;
}
