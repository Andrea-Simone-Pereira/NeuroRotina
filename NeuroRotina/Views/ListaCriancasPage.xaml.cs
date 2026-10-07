using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class ListaCriancasPage : ContentPage
{
    private readonly DatabaseService _database;

    public ListaCriancasPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarCriancasAsync();
    }

    private async Task CarregarCriancasAsync()
    {
        ListaCriancas.Children.Clear();

        if (SessaoUsuario.UsuarioId == 0)
        {
            ListaCriancas.Children.Add(
                new Label
                {
                    Text = "Nenhum adulto está logado.",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        List<Crianca> criancas =
            await _database.BuscarCriancasPorUsuarioAsync(
                SessaoUsuario.UsuarioId);

        if (criancas.Count == 0)
        {
            ListaCriancas.Children.Add(
                new Label
                {
                    Text = "Nenhuma criança cadastrada.",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        foreach (Crianca crianca in criancas)
        {
            Button botaoCrianca = new Button
            {
                Text = crianca.Nome,
                FontSize = 20
            };

            botaoCrianca.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(RotinaPage)}?criancaId={crianca.Id}");
            };

            ListaCriancas.Children.Add(botaoCrianca);
        }
    }

    private async void BtnNovaCrianca_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CriancaPage));
    }
}