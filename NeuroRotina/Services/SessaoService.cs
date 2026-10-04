using NeuroRotina.Models;

namespace NeuroRotina.Services;

/// <summary>
/// Guarda quem está usando o aplicativo no momento e qual criança está selecionada.
/// Registrado como singleton, então o estado é compartilhado por todas as telas.
/// </summary>
public class SessaoService
{
    private const string ChaveUsuario = "usuario_logado";
    private const string ChaveCrianca = "crianca_selecionada";

    public Usuario? UsuarioAtual { get; private set; }
    public Crianca? CriancaAtual { get; set; }

    public bool Autenticado => UsuarioAtual is not null;

    public event EventHandler? CriancaAlterada;

    public void Entrar(Usuario usuario, bool lembrar)
    {
        UsuarioAtual = usuario;

        if (lembrar)
            Preferences.Default.Set(ChaveUsuario, usuario.IdUsuario);
    }

    public void Sair()
    {
        UsuarioAtual = null;
        CriancaAtual = null;
        Preferences.Default.Remove(ChaveUsuario);
        Preferences.Default.Remove(ChaveCrianca);
    }

    public void SelecionarCrianca(Crianca? crianca)
    {
        CriancaAtual = crianca;

        if (crianca is not null)
            Preferences.Default.Set(ChaveCrianca, crianca.IdCrianca);

        CriancaAlterada?.Invoke(this, EventArgs.Empty);
    }

    public int IdCriancaSalvo => Preferences.Default.Get(ChaveCrianca, 0);

    /// <summary>
    /// Se a criança ainda não foi escolhida, seleciona a última usada
    /// ou a primeira da lista do cuidador.
    /// </summary>
    public async Task GarantirCriancaAsync(DatabaseService banco)
    {
        if (CriancaAtual is not null || UsuarioAtual is null)
            return;

        var criancas = await banco.ListarCriancasAsync(UsuarioAtual.IdUsuario);
        if (criancas.Count == 0)
            return;

        var salva = criancas.FirstOrDefault(c => c.IdCrianca == IdCriancaSalvo);
        SelecionarCrianca(salva ?? criancas[0]);
    }
}
