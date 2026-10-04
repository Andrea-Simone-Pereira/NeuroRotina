using System.Collections.ObjectModel;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;
    private readonly AlertaService _alertas;
    private readonly ObservableCollection<Rotina> _rotinas = new();

    public HomePage(DatabaseService banco, SessaoService sessao, AlertaService alertas)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;
        _alertas = alertas;

        BindableLayout.SetItemsSource(ListaRotinas, _rotinas);
        _alertas.AlertaDisparado += AoDispararAlerta;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAsync();
    }

    private async Task CarregarAsync()
    {
        await _sessao.GarantirCriancaAsync(_banco);

        var crianca = _sessao.CriancaAtual;
        _rotinas.Clear();

        if (crianca is null)
        {
            AvatarCrianca.Text = "➕";
            NomeCrianca.Text = "Nenhuma criança cadastrada";
            ResumoCrianca.Text = "Abra o Perfil para cadastrar";
            BotaoModoCrianca.IsEnabled = false;
            return;
        }

        AvatarCrianca.Text = crianca.Avatar;
        NomeCrianca.Text = crianca.Nome;
        ResumoCrianca.Text = crianca.Resumo;

        foreach (var rotina in await _banco.ListarRotinasAsync(crianca.IdCrianca))
            if (rotina.Ativa)
                _rotinas.Add(rotina);

        AtualizarProxima();
    }

    /// <summary>
    /// Mostra a próxima rotina ainda não iniciada. Depois da última do dia,
    /// volta a apontar para a primeira de amanhã.
    /// </summary>
    private void AtualizarProxima()
    {
        var agora = DateTime.Now.ToString("HH:mm");

        var proxima = _rotinas.FirstOrDefault(r => string.CompareOrdinal(r.Horario, agora) >= 0)
                      ?? _rotinas.FirstOrDefault();

        if (proxima is null)
        {
            IconeProxima.Text = "🌙";
            TituloProxima.Text = "Nada programado";
            HorarioProxima.Text = "Crie a primeira rotina na aba Rotinas";
            BotaoModoCrianca.IsEnabled = false;
            return;
        }

        var amanha = string.CompareOrdinal(proxima.Horario, agora) < 0;

        IconeProxima.Text = proxima.Icone;
        TituloProxima.Text = proxima.Titulo;
        HorarioProxima.Text = amanha
            ? $"Amanhã às {proxima.Horario} · {proxima.Legenda}"
            : $"Hoje às {proxima.Horario} · {proxima.Legenda}";

        BotaoModoCrianca.IsEnabled = true;
        BotaoModoCrianca.CommandParameter = proxima.IdRotina;
    }

    private async void AoAtualizar(object? sender, EventArgs e)
    {
        await CarregarAsync();
        Atualizador.IsRefreshing = false;
    }

    private async void AoTocarRotina(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not Rotina rotina)
            return;

        await Shell.Current.GoToAsync($"{nameof(ModoCriancaPage)}?id={rotina.IdRotina}");
    }

    private async void AoAbrirModoCrianca(object? sender, EventArgs e)
    {
        if (BotaoModoCrianca.CommandParameter is not int idRotina)
            return;

        await Shell.Current.GoToAsync($"{nameof(ModoCriancaPage)}?id={idRotina}");
    }

    private async void AoTrocarCrianca(object? sender, EventArgs e)
    {
        if (_sessao.UsuarioAtual is null)
            return;

        var criancas = await _banco.ListarCriancasAsync(_sessao.UsuarioAtual.IdUsuario);

        if (criancas.Count == 0)
        {
            await Shell.Current.GoToAsync($"{nameof(CriancaEditarPage)}?id=0");
            return;
        }

        var nomes = criancas.Select(c => $"{c.Avatar} {c.Nome}").ToArray();
        var escolha = await DisplayActionSheetAsync("Acompanhar quem?", "Cancelar", null, nomes);

        if (escolha is null || escolha == "Cancelar")
            return;

        var indice = Array.IndexOf(nomes, escolha);
        if (indice < 0)
            return;

        _sessao.SelecionarCrianca(criancas[indice]);
        await CarregarAsync();
    }

    private async void AoAbrirHistorico(object? sender, EventArgs e)
    {
        if (_sessao.CriancaAtual is null)
        {
            await DisplayAlertAsync("Cadastre uma criança",
                "O histórico registra o comportamento de uma criança específica.", "Entendi");
            return;
        }

        await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }

    private void AoDispararAlerta(object? sender, AlertaDisparadoEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TextoAlerta.Text = e.Mensagem;
            PainelAlerta.IsVisible = true;
        });
    }

    private void AoFecharAlerta(object? sender, EventArgs e)
    {
        PainelAlerta.IsVisible = false;
    }
}
