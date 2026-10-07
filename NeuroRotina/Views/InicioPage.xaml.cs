namespace NeuroRotina.Views;

public partial class InicioPage : ContentPage
{
    public InicioPage()
    {
        InitializeComponent();
    }

    private async void BtnMinhasCriancas_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(ListaCriancasPage));
    }

    private async void BtnCadastrarCrianca_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(CriancaPage));
    }

    private async void BtnConfiguracoes_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "CONFIGURAÇÕES",
            "A área de configurações será criada posteriormente.",
            "OK");
    }
}