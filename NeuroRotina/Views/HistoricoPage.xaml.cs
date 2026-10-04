using System.Collections.ObjectModel;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

public partial class HistoricoPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;
    private readonly ObservableCollection<Historico> _registros = new();

    public HistoricoPage(DatabaseService banco, SessaoService sessao)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;

        CampoComportamento.ItemsSource = Pictogramas.Comportamentos;
        CampoComportamento.SelectedIndex = 0;
        Lista.ItemsSource = _registros;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAsync();
    }

    private async Task CarregarAsync()
    {
        var crianca = _sessao.CriancaAtual;
        if (crianca is null)
            return;

        Cabecalho.Text = $"Como foi o dia de {crianca.Nome}?";

        _registros.Clear();
        foreach (var registro in await _banco.ListarHistoricoAsync(crianca.IdCrianca))
            _registros.Add(registro);
    }

    private async void AoRegistrar(object? sender, EventArgs e)
    {
        var crianca = _sessao.CriancaAtual;
        if (crianca is null)
            return;

        var registro = new Historico
        {
            IdCrianca = crianca.IdCrianca,
            Data = DateTime.Now,
            Comportamento = CampoComportamento.SelectedItem?.ToString() ?? "Tranquila",
            Observacao = CampoObservacao.Text?.Trim() ?? string.Empty
        };

        await _banco.RegistrarComportamentoAsync(registro);

        CampoObservacao.Text = string.Empty;
        CampoComportamento.SelectedIndex = 0;

        await CarregarAsync();
    }

    private async void AoTocarRegistro(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not Historico registro)
            return;

        var confirmar = await DisplayAlertAsync("Excluir registro",
            $"Apagar o registro de {registro.DataFormatada}?", "Excluir", "Cancelar");

        if (!confirmar)
            return;

        await _banco.ExcluirHistoricoAsync(registro.IdHistorico);
        await CarregarAsync();
    }
}
