using NeuroRotina.Views;

namespace NeuroRotina;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegistrarRotas();
    }

    /// <summary>
    /// Telas abertas a partir de outra tela, sempre com o construtor resolvido
    /// pela injeção de dependência configurada em MauiProgram.
    /// </summary>
    private static void RegistrarRotas()
    {
        Routing.RegisterRoute(nameof(CadastroPage), typeof(CadastroPage));
        Routing.RegisterRoute(nameof(CriancaEditarPage), typeof(CriancaEditarPage));
        Routing.RegisterRoute(nameof(RotinaEditarPage), typeof(RotinaEditarPage));
        Routing.RegisterRoute(nameof(AtividadesPage), typeof(AtividadesPage));
        Routing.RegisterRoute(nameof(AtividadeEditarPage), typeof(AtividadeEditarPage));
        Routing.RegisterRoute(nameof(ModoCriancaPage), typeof(ModoCriancaPage));
        Routing.RegisterRoute(nameof(CartaoEditarPage), typeof(CartaoEditarPage));
        Routing.RegisterRoute(nameof(HistoricoPage), typeof(HistoricoPage));
    }
}
