using System.Collections.ObjectModel;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

/// <summary>
/// Execução da rotina pela criança: um passo por vez, com pictograma grande,
/// fala em voz alta e confirmação por toque.
/// </summary>
[QueryProperty(nameof(IdRotina), "id")]
public partial class ModoCriancaPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly MidiaService _midia;
    private readonly ObservableCollection<Atividade> _passos = new();

    private int _indice;

    public ModoCriancaPage(DatabaseService banco, MidiaService midia)
    {
        InitializeComponent();
        _banco = banco;
        _midia = midia;
        ListaProgresso.ItemsSource = _passos;
    }

    public int IdRotina { get; set; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _passos.Clear();
        foreach (var passo in await _banco.ListarAtividadesAsync(IdRotina))
            _passos.Add(passo);

        _indice = 0;
        MostrarPasso(falar: true);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _midia.PararFala();
    }

    private void MostrarPasso(bool falar)
    {
        if (_passos.Count == 0)
        {
            NomePasso.Text = "Nenhum passo cadastrado";
            IconePasso.Text = "🗂️";
            BotaoPronto.IsEnabled = false;
            BotaoVoltar.IsEnabled = false;
            Contador.Text = string.Empty;
            return;
        }

        var passo = _passos[_indice];

        NomePasso.Text = passo.NomeAtividade;
        IconePasso.Text = passo.Icone;
        Contador.Text = $"{_indice + 1} de {_passos.Count}";

        var temImagem = passo.TemImagem;
        ImagemPasso.Source = temImagem ? ImageSource.FromFile(passo.Imagem) : null;
        ImagemPasso.IsVisible = temImagem;
        IconePasso.IsVisible = !temImagem;

        BotaoVoltar.IsEnabled = _indice > 0;
        BotaoPronto.IsEnabled = true;

        if (falar)
            _ = _midia.FalarAsync(passo.TextoFalado);
    }

    /// <summary>Tocar no pictograma repete o áudio, sem avançar a rotina.</summary>
    private async void AoTocarPasso(object? sender, TappedEventArgs e)
    {
        if (_passos.Count == 0)
            return;

        _midia.Vibrar();
        await _midia.FalarAsync(_passos[_indice].TextoFalado);
    }

    private async void AoConcluirPasso(object? sender, EventArgs e)
    {
        if (_passos.Count == 0)
            return;

        var passo = _passos[_indice];
        passo.Concluida = true;
        await _banco.SalvarAtividadeAsync(passo);

        _midia.Vibrar(80);

        if (_indice < _passos.Count - 1)
        {
            _indice++;
            MostrarPasso(falar: true);
            return;
        }

        await ConcluirRotinaAsync();
    }

    private async Task ConcluirRotinaAsync()
    {
        PainelFim.IsVisible = true;
        await _midia.FalarAsync("Muito bem! Rotina concluída.");
        await _banco.ReiniciarAtividadesAsync(IdRotina);
    }

    private void AoVoltarPasso(object? sender, EventArgs e)
    {
        if (_indice == 0)
            return;

        _indice--;
        MostrarPasso(falar: true);
    }

    private async void AoSair(object? sender, EventArgs e)
    {
        _midia.PararFala();
        await Shell.Current.GoToAsync("..");
    }
}
