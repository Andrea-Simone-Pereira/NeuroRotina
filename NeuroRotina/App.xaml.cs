namespace NeuroRotina;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // O aplicativo é sempre claro: alternância de tema muda o contraste dos
        // pictogramas e prejudica o reconhecimento pela criança.
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell())
        {
            Title = "NeuroRotina"
        };
    }
}
