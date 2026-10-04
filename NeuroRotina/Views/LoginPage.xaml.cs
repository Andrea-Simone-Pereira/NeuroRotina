using NeuroRotina.Services;

namespace NeuroRotina.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;
    private readonly AlertaService _alertas;

    public LoginPage(DatabaseService banco, SessaoService sessao, AlertaService alertas)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;
        _alertas = alertas;
    }

    private async void AoEntrar(object? sender, EventArgs e)
    {
        var email = CampoEmail.Text?.Trim() ?? string.Empty;
        var senha = CampoSenha.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            MostrarErro("Preencha o e-mail e a senha.");
            return;
        }

        DefinirCarregando(true);

        try
        {
            var usuario = await _banco.AutenticarAsync(email, senha);

            if (usuario is null)
            {
                MostrarErro("E-mail ou senha não conferem. Verifique e tente de novo.");
                return;
            }

            _sessao.Entrar(usuario, CampoLembrar.IsChecked);
            await _sessao.GarantirCriancaAsync(_banco);
            _alertas.Iniciar();

            CampoSenha.Text = string.Empty;
            RotuloErro.IsVisible = false;

            await Shell.Current.GoToAsync("//principal/hoje");
        }
        finally
        {
            DefinirCarregando(false);
        }
    }

    private async void AoCriarConta(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroPage));
    }

    private void MostrarErro(string mensagem)
    {
        RotuloErro.Text = mensagem;
        RotuloErro.IsVisible = true;
    }

    private void DefinirCarregando(bool carregando)
    {
        Carregando.IsRunning = carregando;
        Carregando.IsVisible = carregando;
        BotaoEntrar.IsEnabled = !carregando;
    }
}
