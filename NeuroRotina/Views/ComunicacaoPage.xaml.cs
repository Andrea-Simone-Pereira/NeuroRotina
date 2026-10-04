using System.Collections.ObjectModel;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

/// <summary>
/// Prancha de comunicação por imagens. A criança toca um cartão e o aplicativo
/// fala a frase, funcionando como voz de apoio.
/// </summary>
public partial class ComunicacaoPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;
    private readonly MidiaService _midia;

    private readonly ObservableCollection<Cartao> _visiveis = new();
    private List<Cartao> _todos = new();
    private string _categoriaAtual = "Todos";

    public ComunicacaoPage(DatabaseService banco, SessaoService sessao, MidiaService midia)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;
        _midia = midia;
        Lista.ItemsSource = _visiveis;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _midia.PararFala();
    }

    private async Task CarregarAsync()
    {
        await _sessao.GarantirCriancaAsync(_banco);

        _todos = _sessao.CriancaAtual is null
            ? new List<Cartao>()
            : await _banco.ListarCartoesAsync(_sessao.CriancaAtual.IdCrianca);

        MontarFiltros();
        AplicarFiltro(_categoriaAtual);
    }

    /// <summary>Monta um botão por categoria existente, mais "Todos".</summary>
    private void MontarFiltros()
    {
        Filtros.Clear();

        var categorias = new List<string> { "Todos" };
        categorias.AddRange(_todos.Select(c => c.Categoria).Distinct().OrderBy(c => c));

        foreach (var categoria in categorias)
        {
            var selecionada = categoria == _categoriaAtual;

            var botao = new Button
            {
                Text = categoria,
                FontSize = 14,
                HeightRequest = 40,
                Padding = new Thickness(16, 0),
                CornerRadius = 20,
                BackgroundColor = selecionada
                    ? (Color)Application.Current!.Resources["Primaria"]
                    : (Color)Application.Current!.Resources["PrimariaClara"],
                TextColor = selecionada
                    ? Colors.White
                    : (Color)Application.Current!.Resources["Primaria"]
            };

            botao.Clicked += (_, _) =>
            {
                _categoriaAtual = categoria;
                MontarFiltros();
                AplicarFiltro(categoria);
            };

            Filtros.Add(botao);
        }
    }

    private void AplicarFiltro(string categoria)
    {
        _visiveis.Clear();

        var filtrados = categoria == "Todos"
            ? _todos
            : _todos.Where(c => c.Categoria == categoria).ToList();

        foreach (var cartao in filtrados)
            _visiveis.Add(cartao);
    }

    private async void AoTocarCartao(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not Cartao cartao)
            return;

        EcoIcone.Text = cartao.Icone;
        EcoTexto.Text = cartao.Texto;

        _midia.Vibrar();
        await _midia.FalarAsync(cartao.Texto);
    }

    private async void AoGerenciar(object? sender, EventArgs e)
    {
        if (_sessao.CriancaAtual is null)
        {
            await DisplayAlertAsync("Cadastre uma criança",
                "Os cartões pertencem a uma criança. Abra o Perfil para cadastrar.", "Entendi");
            return;
        }

        var acoes = _todos.Select(c => $"{c.Icone} {c.Texto}").ToArray();
        var escolha = await DisplayActionSheetAsync("Qual cartão editar?", "Cancelar", null,
            new[] { "➕ Novo cartão" }.Concat(acoes).ToArray());

        if (escolha is null || escolha == "Cancelar")
            return;

        if (escolha.StartsWith("➕"))
        {
            await Shell.Current.GoToAsync($"{nameof(CartaoEditarPage)}?id=0");
            return;
        }

        var indice = Array.IndexOf(acoes, escolha);
        if (indice < 0)
            return;

        await Shell.Current.GoToAsync($"{nameof(CartaoEditarPage)}?id={_todos[indice].IdCartao}");
    }
}
