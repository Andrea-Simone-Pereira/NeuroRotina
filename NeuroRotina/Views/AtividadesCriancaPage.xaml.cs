using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class AtividadesCriancaPage : ContentPage
{
    private readonly DatabaseService _database;

    public AtividadesCriancaPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarAtividadesAsync();
    }

    private async Task CarregarAtividadesAsync()
    {
        ListaAtividades.Children.Clear();

        if (SessaoCrianca.CriancaId == 0)
        {
            ListaAtividades.Children.Add(
                new Label
                {
                    Text = "NENHUMA CRIANÇA LOGADA.",
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        List<Rotina> rotinas =
            await _database.BuscarRotinasPorCriancaAsync(
                SessaoCrianca.CriancaId);

        if (rotinas.Count == 0)
        {
            ListaAtividades.Children.Add(
                new Label
                {
                    Text = "NENHUMA ATIVIDADE POR ENQUANTO.",
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                });

            return;
        }

        foreach (Rotina rotina in rotinas)
        {
            List<Atividade> atividades =
                await _database.BuscarAtividadesPorRotinaAsync(
                    rotina.Id);

            foreach (Atividade atividade in atividades)
            {
                var botaoAtividade = new Button
                {
                    Text = atividade.Descricao.ToUpper(),
                    FontSize = 22,
                    HeightRequest = 90
                };

                ListaAtividades.Children.Add(botaoAtividade);
            }
        }
    }
}