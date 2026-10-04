using Microsoft.Extensions.Logging;
using NeuroRotina.Services;
using NeuroRotina.Views;

namespace NeuroRotina;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                // Sem fonte personalizada: a fonte do sistema já é a mais legível
                // e respeita o tamanho de texto configurado pelo usuário.
            });

        // Serviços compartilhados por todo o aplicativo
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<SessaoService>();
        builder.Services.AddSingleton<MidiaService>();
        builder.Services.AddSingleton<AlertaService>();

        // Telas
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CadastroPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<RotinasPage>();
        builder.Services.AddTransient<RotinaEditarPage>();
        builder.Services.AddTransient<AtividadesPage>();
        builder.Services.AddTransient<AtividadeEditarPage>();
        builder.Services.AddTransient<ComunicacaoPage>();
        builder.Services.AddTransient<CartaoEditarPage>();
        builder.Services.AddTransient<ModoCriancaPage>();
        builder.Services.AddTransient<PerfilPage>();
        builder.Services.AddTransient<CriancaEditarPage>();
        builder.Services.AddTransient<HistoricoPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
