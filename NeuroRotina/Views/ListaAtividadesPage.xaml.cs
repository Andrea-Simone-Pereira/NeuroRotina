using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

[QueryProperty(nameof(RotinaId), "rotinaId")]
public partial class ListaAtividadesPage : ContentPage
{
    private readonly DatabaseService _database;

    public int RotinaId { get; set; }

    public ListaAtividadesPage()
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

        List<Atividade> atividades =
            await _database.BuscarAtividadesPorRotinaAsync(RotinaId);

        if (atividades.Count == 0)
        {
            ListaAtividades.Children.Add(
                new Label
                {
                    Text = "Nenhuma atividade cadastrada.",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        foreach (Atividade atividade in atividades)
        {
            var blocoAtividade = new Frame
            {
                Padding = 15,
                CornerRadius = 10
            };

            var layoutAtividade = new VerticalStackLayout
            {
                Spacing = 10
            };

            var labelDescricao = new Label
            {
                Text = atividade.Descricao,
                FontSize = 18
            };

            var linhaConclusao = new HorizontalStackLayout
            {
                Spacing = 5
            };

            var checkBox = new CheckBox
            {
                IsChecked = atividade.Concluida
            };

            var labelConcluida = new Label
            {
                Text = "CONCLUÍDA",
                VerticalOptions = LayoutOptions.Center
            };

            checkBox.CheckedChanged += async (sender, args) =>
            {
                atividade.Concluida = args.Value;

                await _database.AtualizarAtividadeAsync(atividade);
            };

            linhaConclusao.Children.Add(checkBox);
            linhaConclusao.Children.Add(labelConcluida);

            layoutAtividade.Children.Add(labelDescricao);
            layoutAtividade.Children.Add(linhaConclusao);

            blocoAtividade.Content = layoutAtividade;

            ListaAtividades.Children.Add(blocoAtividade);
        }
    }
}