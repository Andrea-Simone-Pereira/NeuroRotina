using NeuroRotina.Data;

namespace NeuroRotina;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

        InicializarBanco();
    }

    private async void InicializarBanco()
    {
        var database = new DatabaseService();

        await database.InicializarAsync();
    }
}