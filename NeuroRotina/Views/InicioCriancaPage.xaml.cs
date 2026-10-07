using NeuroRotina.Models;

namespace NeuroRotina.Views;

public partial class InicioCriancaPage : ContentPage
{
    public InicioCriancaPage()
    {
        InitializeComponent();

        lblNomeCrianca.Text =
            $"OLÁ, {SessaoCrianca.Nome.ToUpper()}!";
    }

    private async void BtnAtividades_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(AtividadesCriancaPage));
    }

    private async void BtnNecessidades_Clicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "NECESSIDADES",
            "A tela de necessidades será criada em seguida.",
            "OK");
    }
}