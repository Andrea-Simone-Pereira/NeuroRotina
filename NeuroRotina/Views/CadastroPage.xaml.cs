using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

public partial class CadastroPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;

    public CadastroPage(DatabaseService banco, SessaoService sessao)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;
        CampoTipo.SelectedIndex = 0;
    }

    private async void AoSalvar(object? sender, EventArgs e)
    {
        var nome = CampoNome.Text?.Trim() ?? string.Empty;
        var email = CampoEmail.Text?.Trim() ?? string.Empty;
        var senha = CampoSenha.Text ?? string.Empty;
        var confirmacao = CampoConfirmacao.Text ?? string.Empty;

        var erro = Validar(nome, email, senha, confirmacao);
        if (erro is not null)
        {
            MostrarErro(erro);
            return;
        }

        if (await _banco.EmailJaCadastradoAsync(email))
        {
            MostrarErro("Já existe uma conta com esse e-mail. Entre com ela ou use outro endereço.");
            return;
        }

        BotaoSalvar.IsEnabled = false;

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Senha = SenhaHelper.Hash(senha),
            TipoUsuario = CampoTipo.SelectedItem?.ToString() ?? "Cuidador"
        };

        await _banco.SalvarUsuarioAsync(usuario);
        _sessao.Entrar(usuario, lembrar: true);

        await DisplayAlertAsync("Conta criada",
            "Agora cadastre a criança que vai usar o aplicativo.", "Continuar");

        await Shell.Current.GoToAsync("//principal/perfil");
        await Shell.Current.GoToAsync($"{nameof(CriancaEditarPage)}?id=0");
    }

    private static string? Validar(string nome, string email, string senha, string confirmacao)
    {
        if (nome.Length < 3)
            return "Informe o nome completo.";

        if (!email.Contains('@') || !email.Contains('.'))
            return "Informe um e-mail válido.";

        if (senha.Length < 6)
            return "A senha precisa de pelo menos 6 caracteres.";

        if (senha != confirmacao)
            return "As duas senhas precisam ser iguais.";

        return null;
    }

    private void MostrarErro(string mensagem)
    {
        RotuloErro.Text = mensagem;
        RotuloErro.IsVisible = true;
        BotaoSalvar.IsEnabled = true;
    }
}
