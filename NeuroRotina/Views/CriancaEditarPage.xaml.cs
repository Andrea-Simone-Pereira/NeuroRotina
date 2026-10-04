using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

[QueryProperty(nameof(IdCrianca), "id")]
public partial class CriancaEditarPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;

    private Crianca _crianca = new();

    public CriancaEditarPage(DatabaseService banco, SessaoService sessao)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;

        ListaAvatares.ItemsSource = Pictogramas.Avatares;
        CampoNivel.ItemsSource = Pictogramas.NiveisSuporte;
    }

    public int IdCrianca { get; set; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (IdCrianca > 0)
        {
            _crianca = await _banco.ObterCriancaAsync(IdCrianca) ?? NovaCrianca();
            Title = "Editar criança";
            BotaoExcluir.IsVisible = true;
        }
        else
        {
            _crianca = NovaCrianca();
            Title = "Nova criança";
            BotaoExcluir.IsVisible = false;
        }

        CampoNome.Text = _crianca.Nome;
        CampoIdade.Text = _crianca.Idade > 0 ? _crianca.Idade.ToString() : string.Empty;
        CampoNivel.SelectedItem = _crianca.NivelSuporte;
        ListaAvatares.SelectedItem = _crianca.Avatar;
    }

    private Crianca NovaCrianca() => new()
    {
        IdCuidador = _sessao.UsuarioAtual?.IdUsuario ?? 0
    };

    private void AoEscolherAvatar(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string avatar)
            _crianca.Avatar = avatar;
    }

    private async void AoSalvar(object? sender, EventArgs e)
    {
        var nome = CampoNome.Text?.Trim() ?? string.Empty;

        if (nome.Length < 2)
        {
            await DisplayAlertAsync("Falta o nome", "Informe como a criança é chamada.", "Entendi");
            return;
        }

        _crianca.Nome = nome;
        _crianca.NivelSuporte = CampoNivel.SelectedItem?.ToString() ?? "Nível 1";
        _crianca.Idade = int.TryParse(CampoIdade.Text, out var idade) ? idade : 0;

        var primeira = _crianca.IdCrianca == 0;
        await _banco.SalvarCriancaAsync(_crianca);

        // A criança recém-cadastrada passa a ser a acompanhada.
        if (primeira || _sessao.CriancaAtual is null)
            _sessao.SelecionarCrianca(_crianca);

        await Shell.Current.GoToAsync("..");
    }

    private async void AoExcluir(object? sender, EventArgs e)
    {
        var confirmar = await DisplayAlertAsync("Excluir criança",
            $"Todas as rotinas, cartões e registros de {_crianca.Nome} serão apagados.",
            "Excluir", "Cancelar");

        if (!confirmar)
            return;

        await _banco.ExcluirCriancaAsync(_crianca.IdCrianca);

        if (_sessao.CriancaAtual?.IdCrianca == _crianca.IdCrianca)
            _sessao.SelecionarCrianca(null);

        await Shell.Current.GoToAsync("..");
    }
}
