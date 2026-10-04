using System.Linq;
namespace NeuroRotina.Services;

/// <summary>
/// Reúne os recursos multimídia do aplicativo: fala em voz alta (síntese de voz
/// nativa), seleção de imagens da galeria e vibração de apoio ao toque.
/// </summary>
public class MidiaService
{
    private Locale? _vozPortugues;
    private CancellationTokenSource? _falaAtual;

    /// <summary>
    /// Fala o texto em português. Uma nova fala interrompe a anterior, evitando
    /// sobreposição de áudio quando a criança toca vários cartões seguidos.
    /// </summary>
    public async Task FalarAsync(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return;

        try
        {
            _falaAtual?.Cancel();
            _falaAtual = new CancellationTokenSource();

            _vozPortugues ??= (await TextToSpeech.Default.GetLocalesAsync())
                .FirstOrDefault(l => l.Language.StartsWith("pt", StringComparison.OrdinalIgnoreCase));

            var opcoes = new SpeechOptions
            {
                Locale = _vozPortugues,
                Pitch = 1.0f,
                Volume = 1.0f
            };

            await TextToSpeech.Default.SpeakAsync(texto, opcoes, _falaAtual.Token);
        }
        catch (OperationCanceledException)
        {
            // Fala substituída por outra: comportamento esperado.
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[NeuroRotina] Falha na sintese de voz: {ex.Message}");
        }
    }

    public void PararFala()
    {
        _falaAtual?.Cancel();
        _falaAtual = null;
    }

    /// <summary>Retorno tátil curto ao confirmar um toque.</summary>
    public void Vibrar(int milissegundos = 60)
    {
        try
        {
            if (Vibration.Default.IsSupported)
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(milissegundos));
        }
        catch (FeatureNotSupportedException)
        {
            // Dispositivo sem vibração: ignorar.
        }
    }

    /// <summary>
    /// Abre a galeria, copia a imagem escolhida para a pasta do aplicativo e
    /// devolve o caminho local. Copiar é necessário porque o caminho original
    /// pode deixar de ser acessível depois.
    /// </summary>
    public async Task<string?> EscolherImagemAsync()
    {
        try
        {
            var arquivos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Escolha uma imagem"
            });

            var arquivo = arquivos?.FirstOrDefault();
            if (arquivo is null)
                return null;

            var pasta = Path.Combine(FileSystem.AppDataDirectory, "imagens");
            Directory.CreateDirectory(pasta);

            var destino = Path.Combine(pasta, $"{Guid.NewGuid():N}{Path.GetExtension(arquivo.FileName)}");

            await using var origem = await arquivo.OpenReadAsync();
            await using var saida = File.Create(destino);
            await origem.CopyToAsync(saida);

            return destino;
        }
        catch (PermissionException)
        {
            await MostrarAvisoAsync("Permissão negada",
                "Autorize o acesso às fotos nas configurações do aparelho para usar imagens próprias.");
            return null;
        }
        catch (Exception ex)
        {
            await MostrarAvisoAsync("Não foi possível abrir a galeria", ex.Message);
            return null;
        }
    }

    private static Task MostrarAvisoAsync(string titulo, string mensagem)
    {
        var janela = Application.Current?.Windows.FirstOrDefault();
        return janela?.Page?.DisplayAlertAsync(titulo, mensagem, "Entendi") ?? Task.CompletedTask;
    }
}
