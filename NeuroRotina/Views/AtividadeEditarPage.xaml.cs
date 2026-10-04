using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

[QueryProperty(nameof(IdAtividade), "id")]
[QueryProperty(nameof(IdRotina), "rotina")]
public partial class AtividadeEditarPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly MidiaService _midia;

    private Atividade _atividade = new();

    public AtividadeEditarPage(DatabaseService banco, MidiaService midia)
    {
        InitializeComponent();
        _banco = banco;
        _midia = midia;
        ListaIcones.ItemsSource = Pictogramas.Atividades;
    }

    public int IdAtividade { get; set; }
    public int IdRotina { get; set; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (IdAtividade > 0)
        {
            _atividade = await _banco.ObterAtividadeAsync(IdAtividade) ?? new Atividade { IdRotina = IdRotina };
            Title = "Editar passo";
            BotaoExcluir.IsVisible = true;
        }
        else
        {
            _atividade = new Atividade { IdRotina = IdRotina };
            Title = "Novo passo";
            BotaoExcluir.IsVisible = false;
        }

        CampoNome.Text = _atividade.NomeAtividade;
        CampoAudio.Text = _atividade.Audio;
        ListaIcones.SelectedItem = _atividade.Icone;

        AtualizarPrevia();
    }

    private void AoMudarNome(object? sender, TextChangedEventArgs e)
    {
        _atividade.NomeAtividade = e.NewTextValue ?? string.Empty;
        AtualizarPrevia();
    }

    private void AoEscolherIcone(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not string icone)
            return;

        _atividade.Icone = icone;
        AtualizarPrevia();
    }

    private async void AoEscolherFoto(object? sender, EventArgs e)
    {
        var caminho = await _midia.EscolherImagemAsync();
        if (caminho is null)
            return;

        _atividade.Imagem = caminho;
        AtualizarPrevia();
    }

    private void AoRemoverFoto(object? sender, EventArgs e)
    {
        _atividade.Imagem = string.Empty;
        AtualizarPrevia();
    }

    private void AtualizarPrevia()
    {
        PreviaTexto.Text = string.IsNullOrWhiteSpace(_atividade.NomeAtividade)
            ? "Novo passo"
            : _atividade.NomeAtividade;

        PreviaIcone.Text = _atividade.Icone;

        var temImagem = _atividade.TemImagem;
        PreviaImagem.Source = temImagem ? ImageSource.FromFile(_atividade.Imagem) : null;
        PreviaImagem.IsVisible = temImagem;
        PreviaIcone.IsVisible = !temImagem;
        BotaoRemoverFoto.IsVisible = temImagem;
    }

    private async void AoOuvir(object? sender, EventArgs e)
    {
        var texto = string.IsNullOrWhiteSpace(CampoAudio.Text)
            ? CampoNome.Text
            : CampoAudio.Text;

        await _midia.FalarAsync(texto);
    }

    private async void AoSalvar(object? sender, EventArgs e)
    {
        var nome = CampoNome.Text?.Trim() ?? string.Empty;

        if (nome.Length < 2)
        {
            await DisplayAlertAsync("Falta o nome", "Escreva o nome do passo com poucas palavras.", "Entendi");
            return;
        }

        _atividade.NomeAtividade = nome;
        _atividade.Audio = CampoAudio.Text?.Trim() ?? string.Empty;

        await _banco.SalvarAtividadeAsync(_atividade);
        await Shell.Current.GoToAsync("..");
    }

    private async void AoExcluir(object? sender, EventArgs e)
    {
        var confirmar = await DisplayAlertAsync("Excluir passo",
            $"O passo “{_atividade.NomeAtividade}” será removido da rotina.", "Excluir", "Cancelar");

        if (!confirmar)
            return;

        await _banco.ExcluirAtividadeAsync(_atividade.IdAtividade);
        await Shell.Current.GoToAsync("..");
    }
}
