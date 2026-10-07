using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

[QueryProperty(nameof(CriancaId), "criancaId")]
public partial class ListaRotinasPage : ContentPage
{
    private readonly DatabaseService _database;

    public int CriancaId { get; set; }

    public ListaRotinasPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarRotinasAsync();
    }

    private async Task CarregarRotinasAsync()
    {
        ListaRotinas.Children.Clear();

        List<Rotina> rotinas =
            await _database.BuscarRotinasPorCriancaAsync(CriancaId);

        if (rotinas.Count == 0)
        {
            ListaRotinas.Children.Add(
                new Label
                {
                    Text = "Nenhuma rotina cadastrada.",
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.Center
                });

            return;
        }

        foreach (Rotina rotina in rotinas)
        {
            var blocoRotina = new Frame
            {
                Padding = 15,
                CornerRadius = 10
            };

            var layoutRotina = new VerticalStackLayout
            {
                Spacing = 10
            };

            var labelTitulo = new Label
            {
                Text = rotina.Titulo.ToUpper(),
                FontSize = 22,
                FontAttributes = FontAttributes.Bold
            };

            var labelDescricao = new Label
            {
                Text = rotina.Descricao,
                FontSize = 16
            };

            var linhaConclusao = new HorizontalStackLayout
            {
                Spacing = 5
            };

            var checkBox = new CheckBox
            {
                IsChecked = rotina.Concluida
            };

            var labelConcluida = new Label
            {
                Text = "Concluída",
                VerticalOptions = LayoutOptions.Center
            };

            checkBox.CheckedChanged += async (sender, args) =>
            {
                rotina.Concluida = args.Value;

                await _database.AtualizarRotinaAsync(rotina);
            };

            linhaConclusao.Children.Add(checkBox);
            linhaConclusao.Children.Add(labelConcluida);

            var botaoEditar = new Button
            {
                Text = "EDITAR ROTINA"
            };

            botaoEditar.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(RotinaPage)}?criancaId={CriancaId}&rotinaId={rotina.Id}");
            };

            var botaoAtividade = new Button
            {
                Text = "ADICIONAR ATIVIDADE"
            };

            botaoAtividade.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(AtividadePage)}?rotinaId={rotina.Id}&rotinaTitulo={Uri.EscapeDataString(rotina.Titulo)}");
            };

            var botaoVerAtividades = new Button
            {
                Text = "VER ATIVIDADES"
            };

            botaoVerAtividades.Clicked += async (sender, args) =>
            {
                await Shell.Current.GoToAsync(
                    $"{nameof(ListaAtividadesPage)}?rotinaId={rotina.Id}");
            };

            layoutRotina.Children.Add(labelTitulo);
            layoutRotina.Children.Add(labelDescricao);
            layoutRotina.Children.Add(linhaConclusao);
            layoutRotina.Children.Add(botaoEditar);
            layoutRotina.Children.Add(botaoAtividade);
            layoutRotina.Children.Add(botaoVerAtividades);

            blocoRotina.Content = layoutRotina;

            ListaRotinas.Children.Add(blocoRotina);
        }
    }
}