using NeuroRotina.Models;
using NeuroRotina.Services;

namespace NeuroRotina.Views;

[QueryProperty(nameof(IdRotina), "id")]
public partial class RotinaEditarPage : ContentPage
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;

    private Rotina _rotina = new();
    private Alerta? _alerta;

    public RotinaEditarPage(DatabaseService banco, SessaoService sessao)
    {
        InitializeComponent();
        _banco = banco;
        _sessao = sessao;

        ListaIcones.ItemsSource = Pictogramas.Rotinas;
        CampoAntecedencia.SelectedIndex = 1;
    }

    public int IdRotina { get; set; }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAsync();
    }

    private async Task CarregarAsync()
    {
        if (IdRotina > 0)
        {
            _rotina = await _banco.ObterRotinaAsync(IdRotina) ?? new Rotina();
            _alerta = await _banco.ObterAlertaDaRotinaAsync(IdRotina);
            Title = "Editar rotina";
            BotaoExcluir.IsVisible = true;
            BotaoAtividades.IsVisible = true;
        }
        else
        {
            _rotina = new Rotina
            {
                IdCrianca = _sessao.CriancaAtual?.IdCrianca ?? 0,
                Horario = "07:00"
            };
            Title = "Nova rotina";
            BotaoExcluir.IsVisible = false;
            BotaoAtividades.IsVisible = false;
        }

        CampoTitulo.Text = _rotina.Titulo;
        CampoDescricao.Text = _rotina.Descricao;
        CampoAtiva.IsToggled = _rotina.Ativa;
        ListaIcones.SelectedItem = _rotina.Icone;

        if (TimeSpan.TryParse(_rotina.Horario, out var hora))
            CampoHorario.Time = hora;

        CampoAlertaAtivo.IsToggled = _alerta?.Ativo ?? false;
        PainelAlerta.IsVisible = CampoAlertaAtivo.IsToggled;

        if (_alerta is not null)
        {
            CampoMensagem.Text = _alerta.Mensagem;
            CampoAntecedencia.SelectedIndex = _alerta.AntecedenciaMinutos switch
            {
                5 => 0,
                15 => 2,
                30 => 3,
                _ => 1
            };
        }

        AtualizarResumoAlerta();
    }

    private void AoEscolherIcone(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string icone)
            _rotina.Icone = icone;
    }

    private void AoAlternarAlerta(object? sender, ToggledEventArgs e)
    {
        PainelAlerta.IsVisible = e.Value;
        AtualizarResumoAlerta();
    }

    private void AoMudarAntecedencia(object? sender, EventArgs e) => AtualizarResumoAlerta();

    private void AtualizarResumoAlerta()
    {
        if (!CampoAlertaAtivo.IsToggled)
            return;

        var horario = (CampoHorario.Time ?? TimeSpan.Zero).ToString(@"hh\:mm");
        var horarioAlerta = AlertaService.CalcularHorario(horario, MinutosSelecionados());
        ResumoAlerta.Text = $"O aviso será dado às {horarioAlerta}, antes da rotina das {horario}.";
    }

    private int MinutosSelecionados() => CampoAntecedencia.SelectedIndex switch
    {
        0 => 5,
        2 => 15,
        3 => 30,
        _ => 10
    };

    private async void AoSalvar(object? sender, EventArgs e)
    {
        var titulo = CampoTitulo.Text?.Trim() ?? string.Empty;

        if (titulo.Length < 2)
        {
            await DisplayAlertAsync("Falta o nome", "Dê um nome curto e sempre igual à rotina.", "Entendi");
            return;
        }

        if (_rotina.IdCrianca == 0)
        {
            await DisplayAlertAsync("Sem criança selecionada",
                "Selecione uma criança na aba Hoje antes de criar a rotina.", "Entendi");
            return;
        }

        _rotina.Titulo = titulo;
        _rotina.Descricao = CampoDescricao.Text?.Trim() ?? string.Empty;
        _rotina.Horario = (CampoHorario.Time ?? TimeSpan.Zero).ToString(@"hh\:mm");
        _rotina.Ativa = CampoAtiva.IsToggled;

        var id = await _banco.SalvarRotinaAsync(_rotina);
        await SalvarAlertaAsync(id);

        await Shell.Current.GoToAsync("..");
    }

    private async Task SalvarAlertaAsync(int idRotina)
    {
        if (!CampoAlertaAtivo.IsToggled && _alerta is null)
            return;

        _alerta ??= new Alerta { IdRotina = idRotina };

        _alerta.Ativo = CampoAlertaAtivo.IsToggled;
        _alerta.AntecedenciaMinutos = MinutosSelecionados();
        _alerta.Horario = AlertaService.CalcularHorario(_rotina.Horario, _alerta.AntecedenciaMinutos);
        _alerta.Mensagem = string.IsNullOrWhiteSpace(CampoMensagem.Text)
            ? $"Daqui a pouco começa: {_rotina.Titulo}"
            : CampoMensagem.Text.Trim();

        await _banco.SalvarAlertaAsync(_alerta);
    }

    private async void AoAbrirAtividades(object? sender, EventArgs e)
    {
        if (_rotina.IdRotina == 0)
            return;

        await Shell.Current.GoToAsync($"{nameof(AtividadesPage)}?id={_rotina.IdRotina}");
    }

    private async void AoExcluir(object? sender, EventArgs e)
    {
        var confirmar = await DisplayAlertAsync("Excluir rotina",
            $"A rotina “{_rotina.Titulo}” e todos os seus passos serão apagados.",
            "Excluir", "Cancelar");

        if (!confirmar)
            return;

        await _banco.ExcluirRotinaAsync(_rotina.IdRotina);
        await Shell.Current.GoToAsync("..");
    }
}
