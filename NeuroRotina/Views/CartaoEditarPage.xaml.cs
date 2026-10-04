using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

[QueryProperty(nameof(IdCartao), "id")]
public partial class CartaoEditarPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;
    private readonly MidiaService _midia;

    private Cartao _cartao = new();

    public CartaoEditarPage(DatabaseService banco, SessaoService sessao, MidiaService midia)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;
        _midia = midia;
        ListaIcones.ItemsSource = Pictogramas.Cartoes;
    }

    public int IdCartao { get; set; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (IdCartao > 0)
        {
            _cartao = await _banco.ObterCartaoAsync(IdCartao) ?? NovoCartao();
            Title = "Editar cartão";
            BotaoExcluir.IsVisible = true;
        }
        else
        {
            _cartao = NovoCartao();
            Title = "Novo cartão";
            BotaoExcluir.IsVisible = false;
        }

        CampoTexto.Text = _cartao.Texto;
        CampoCategoria.SelectedItem = _cartao.Categoria;
        ListaIcones.SelectedItem = _cartao.Icone;

        AtualizarPrevia();
    }

    private Cartao NovoCartao() => new()
    {
        IdCrianca = _sessao.CriancaAtual?.IdCrianca ?? 0,
        Categoria = "Necessidades"
    };

    private void AoMudarTexto(object? sender, TextChangedEventArgs e)
    {
        _cartao.Texto = e.NewTextValue ?? string.Empty;
        AtualizarPrevia();
    }

    private void AoEscolherIcone(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not string icone)
            return;

        _cartao.Icone = icone;
        AtualizarPrevia();
    }

    private async void AoEscolherFoto(object? sender, EventArgs e)
    {
        var caminho = await _midia.EscolherImagemAsync();
        if (caminho is null)
            return;

        _cartao.Imagem = caminho;
        AtualizarPrevia();
    }

    private void AoRemoverFoto(object? sender, EventArgs e)
    {
        _cartao.Imagem = string.Empty;
        AtualizarPrevia();
    }

    private void AtualizarPrevia()
    {
        PreviaTexto.Text = string.IsNullOrWhiteSpace(_cartao.Texto) ? "Novo cartão" : _cartao.Texto;
        PreviaIcone.Text = _cartao.Icone;

        var temImagem = _cartao.TemImagem;
        PreviaImagem.Source = temImagem ? ImageSource.FromFile(_cartao.Imagem) : null;
        PreviaImagem.IsVisible = temImagem;
        PreviaIcone.IsVisible = !temImagem;
        BotaoRemoverFoto.IsVisible = temImagem;
    }

    private async void AoOuvir(object? sender, EventArgs e) => await _midia.FalarAsync(CampoTexto.Text);

    private async void AoSalvar(object? sender, EventArgs e)
    {
        var texto = CampoTexto.Text?.Trim() ?? string.Empty;

        if (texto.Length < 2)
        {
            await DisplayAlertAsync("Falta a frase", "Escreva a frase que o aplicativo vai falar.", "Entendi");
            return;
        }

        if (_cartao.IdCrianca == 0)
        {
            await DisplayAlertAsync("Sem criança selecionada",
                "Selecione uma criança na aba Hoje antes de criar cartões.", "Entendi");
            return;
        }

        _cartao.Texto = texto;
        _cartao.Categoria = CampoCategoria.SelectedItem?.ToString() ?? "Necessidades";

        await _banco.SalvarCartaoAsync(_cartao);
        await Shell.Current.GoToAsync("..");
    }

    private async void AoExcluir(object? sender, EventArgs e)
    {
        var confirmar = await DisplayAlertAsync("Excluir cartão",
            $"O cartão “{_cartao.Texto}” será removido da prancha.", "Excluir", "Cancelar");

        if (!confirmar)
            return;

        await _banco.ExcluirCartaoAsync(_cartao.IdCartao);
        await Shell.Current.GoToAsync("..");
    }
}
