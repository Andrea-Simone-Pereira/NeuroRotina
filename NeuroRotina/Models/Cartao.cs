using SQLite;

namespace NeuroRotina.Models;

/// <summary>
/// Cartão da prancha de comunicação por imagens usada pela criança.
/// Atende à história de usuário 4 do backlog ("botões com imagens para me comunicar").
/// </summary>
[Table("cartao")]
public class Cartao
{
    [PrimaryKey, AutoIncrement]
    public int IdCartao { get; set; }

    [Indexed]
    public int IdCrianca { get; set; }

    [MaxLength(40), NotNull]
    public string Texto { get; set; } = string.Empty;

    [MaxLength(8)]
    public string Icone { get; set; } = "💬";

    public string Imagem { get; set; } = string.Empty;

    [MaxLength(30)]
    public string Categoria { get; set; } = "Necessidades";

    public int Ordem { get; set; }

    [Ignore]
    public bool TemImagem => !string.IsNullOrWhiteSpace(Imagem) && File.Exists(Imagem);
}
