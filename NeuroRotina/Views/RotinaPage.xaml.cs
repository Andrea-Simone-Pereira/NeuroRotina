using NeuroRotina.Data;
using NeuroRotina.Models;

namespace NeuroRotina.Views;

[QueryProperty(nameof(CriancaId), "criancaId")]
[QueryProperty(nameof(RotinaId), "rotinaId")]
public partial class RotinaPage : ContentPage
{
    private readonly DatabaseService _database;

    public int CriancaId { get; set; }

    public int RotinaId { get; set; }

    public RotinaPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (RotinaId > 0)
        {
            await CarregarRotinaAsync();
        }
        else
        {
            lblTituloPagina.Text = "NOVA ROTINA";
            btnSalvar.Text = "CADASTRAR ROTINA";
        }
    }

    private async Task CarregarRotinaAsync()
    {
        List<Rotina> rotinas =
            await _database.BuscarRotinasPorCriancaAsync(CriancaId);

        Rotina? rotina = rotinas
            .FirstOrDefault(r => r.Id == RotinaId);

        if (rotina == null)
        {
            return;
        }

        lblTituloPagina.Text = "EDITAR ROTINA";

        btnSalvar.Text = "SALVAR ALTERAÇÕES";

        txtTitulo.Text = rotina.Titulo;

        txtDescricao.Text = rotina.Descricao;

        dtpData.Date = rotina.Data;
    }

    private async void BtnCadastrar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTitulo.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Informe o título da rotina.",
                "OK");

            return;
        }

        if (RotinaId > 0)
        {
            List<Rotina> rotinas =
                await _database.BuscarRotinasPorCriancaAsync(CriancaId);

            Rotina? rotina = rotinas
                .FirstOrDefault(r => r.Id == RotinaId);

            if (rotina == null)
            {
                await DisplayAlert(
                    "Erro",
                    "Rotina não encontrada.",
                    "OK");

                return;
            }

            rotina.Titulo = txtTitulo.Text.ToUpper();
            rotina.Descricao = txtDescricao.Text ?? string.Empty;
            rotina.Data = dtpData.Date ?? DateTime.Today;

            await _database.AtualizarRotinaAsync(rotina);

            await DisplayAlert(
                "Sucesso",
                "Rotina alterada com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("..");

            return;
        }

        var novaRotina = new Rotina
        {
            CriancaId = CriancaId,
            Titulo = txtTitulo.Text.ToUpper(),
            Descricao = txtDescricao.Text ?? string.Empty,
            Data = dtpData.Date ?? DateTime.Today,
            Concluida = false
        };

        await _database.CadastrarRotinaAsync(novaRotina);

        await DisplayAlert(
            "Sucesso",
            "Rotina cadastrada com sucesso!",
            "OK");

        txtTitulo.Text = string.Empty;
        txtDescricao.Text = string.Empty;
    }

    private async void BtnVerRotinas_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(ListaRotinasPage)}?criancaId={CriancaId}");
    }
}