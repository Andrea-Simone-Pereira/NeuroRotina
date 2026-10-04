using System.Collections.ObjectModel;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

public partial class RotinasPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;
    private readonly ObservableCollection<Rotina> _rotinas = new();

    public RotinasPage(DatabaseService banco, SessaoService sessao)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;
        Lista.ItemsSource = _rotinas;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAsync();
    }

    private async Task CarregarAsync()
    {
        await _sessao.GarantirCriancaAsync(_banco);
        _rotinas.Clear();

        if (_sessao.CriancaAtual is null)
        {
            Cabecalho.Text = "Rotinas";
            return;
        }

        Cabecalho.Text = $"Rotinas de {_sessao.CriancaAtual.Nome}";

        foreach (var rotina in await _banco.ListarRotinasAsync(_sessao.CriancaAtual.IdCrianca))
            _rotinas.Add(rotina);
    }

    private async void AoAtualizar(object? sender, EventArgs e)
    {
        await CarregarAsync();
        Atualizador.IsRefreshing = false;
    }

    private async void AoTocar(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not Rotina rotina)
            return;

        await Shell.Current.GoToAsync($"{nameof(RotinaEditarPage)}?id={rotina.IdRotina}");
    }

    private async void AoCriar(object? sender, EventArgs e)
    {
        if (_sessao.CriancaAtual is null)
        {
            await DisplayAlertAsync("Cadastre uma criança primeiro",
                "Toda rotina pertence a uma criança. Abra o Perfil para cadastrar.", "Entendi");
            return;
        }

        await Shell.Current.GoToAsync($"{nameof(RotinaEditarPage)}?id=0");
    }
}
