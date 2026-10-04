using System.Collections.ObjectModel;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

[QueryProperty(nameof(IdRotina), "id")]
public partial class AtividadesPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly ObservableCollection<Atividade> _atividades = new();

    public AtividadesPage(DatabaseService banco)
    {
        InitializeComponent();
        _banco = banco;
        Lista.ItemsSource = _atividades;
    }

    public int IdRotina { get; set; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAsync();
    }

    private async Task CarregarAsync()
    {
        var rotina = await _banco.ObterRotinaAsync(IdRotina);
        Cabecalho.Text = rotina is null ? "Passos" : $"{rotina.Icone} {rotina.Titulo}";

        _atividades.Clear();
        foreach (var atividade in await _banco.ListarAtividadesAsync(IdRotina))
            _atividades.Add(atividade);
    }

    private async void AoTocar(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not Atividade atividade)
            return;

        await Shell.Current.GoToAsync(
            $"{nameof(AtividadeEditarPage)}?id={atividade.IdAtividade}&rotina={IdRotina}");
    }

    private async void AoCriar(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AtividadeEditarPage)}?id=0&rotina={IdRotina}");
    }

    private async void AoVisualizar(object? sender, EventArgs e)
    {
        if (_atividades.Count == 0)
        {
            await DisplayAlertAsync("Rotina vazia", "Adicione ao menos um passo para visualizar.", "Entendi");
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(ModoCriancaPage)}?id={IdRotina}");
    }

    private async void AoSubir(object? sender, EventArgs e) => await MoverAsync(sender, -1);

    private async void AoDescer(object? sender, EventArgs e) => await MoverAsync(sender, +1);

    /// <summary>
    /// Troca a posição de dois passos e grava a nova ordem no banco.
    /// </summary>
    private async Task MoverAsync(object? sender, int direcao)
    {
        if ((sender as Button)?.CommandParameter is not int id)
            return;

        var atual = _atividades.FirstOrDefault(a => a.IdAtividade == id);
        if (atual is null)
            return;

        var indice = _atividades.IndexOf(atual);
        var destino = indice + direcao;

        if (destino < 0 || destino >= _atividades.Count)
            return;

        var vizinho = _atividades[destino];

        (atual.Ordem, vizinho.Ordem) = (vizinho.Ordem, atual.Ordem);

        await _banco.SalvarAtividadeAsync(atual);
        await _banco.SalvarAtividadeAsync(vizinho);

        _atividades.Move(indice, destino);
    }
}
