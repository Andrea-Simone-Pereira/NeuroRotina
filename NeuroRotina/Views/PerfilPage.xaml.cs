using System.Collections.ObjectModel;
using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

public partial class PerfilPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;
    private readonly AlertaService _alertas;
    private readonly ObservableCollection<Crianca> _criancas = new();

    public PerfilPage(DatabaseService banco, SessaoService sessao, AlertaService alertas)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;
        _alertas = alertas;
        ListaCriancas.ItemsSource = _criancas;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAsync();
    }

    private async Task CarregarAsync()
    {
        var usuario = _sessao.UsuarioAtual;
        if (usuario is null)
            return;

        Iniciais.Text = usuario.Iniciais;
        NomeUsuario.Text = usuario.Nome;
        EmailUsuario.Text = usuario.Email;
        CampoNome.Text = usuario.Nome;
        CampoSenha.Text = string.Empty;

        _criancas.Clear();
        foreach (var crianca in await _banco.ListarCriancasAsync(usuario.IdUsuario))
            _criancas.Add(crianca);
    }

    private async void AoSalvarPerfil(object? sender, EventArgs e)
    {
        var usuario = _sessao.UsuarioAtual;
        if (usuario is null)
            return;

        var nome = CampoNome.Text?.Trim() ?? string.Empty;

        if (nome.Length < 3)
        {
            await DisplayAlertAsync("Nome muito curto", "Informe o nome completo.", "Entendi");
            return;
        }

        var senha = CampoSenha.Text ?? string.Empty;

        if (senha.Length > 0 && senha.Length < 6)
        {
            await DisplayAlertAsync("Senha curta", "A nova senha precisa de pelo menos 6 caracteres.", "Entendi");
            return;
        }

        usuario.Nome = nome;

        if (senha.Length >= 6)
            usuario.Senha = SenhaHelper.Hash(senha);

        await _banco.SalvarUsuarioAsync(usuario);
        await CarregarAsync();

        await DisplayAlertAsync("Perfil atualizado", "Suas alterações foram salvas.", "Ok");
    }

    private async void AoAdicionarCrianca(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(CriancaEditarPage)}?id=0");
    }

    private async void AoTocarCrianca(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not Crianca crianca)
            return;

        await Shell.Current.GoToAsync($"{nameof(CriancaEditarPage)}?id={crianca.IdCrianca}");
    }

    private async void AoAbrirHistorico(object? sender, EventArgs e)
    {
        if (_sessao.CriancaAtual is null)
        {
            await DisplayAlertAsync("Cadastre uma criança",
                "O histórico é registrado por criança.", "Entendi");
            return;
        }

        await Shell.Current.GoToAsync(nameof(HistoricoPage));
    }

    private async void AoSair(object? sender, EventArgs e)
    {
        var confirmar = await DisplayAlertAsync("Sair da conta",
            "Você precisará entrar novamente com e-mail e senha.", "Sair", "Cancelar");

        if (!confirmar)
            return;

        _alertas.Parar();
        _sessao.Sair();
        await Shell.Current.GoToAsync("//login");
    }
}
