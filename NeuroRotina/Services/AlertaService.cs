using NeuroRotina.Models;

namespace NeuroRotina.Services;

/// <summary>
/// Verifica a cada minuto se algum alerta configurado chegou no horário e,
/// nesse caso, avisa o cuidador e fala a mensagem em voz alta.
/// Sprint 5 do backlog: sistema de alertas e notificações visuais.
/// </summary>
public class AlertaService
{
    private readonly DatabaseService _banco;
    private readonly SessaoService _sessao;
    private readonly MidiaService _midia;
    private IDispatcherTimer? _relogio;

    public AlertaService(DatabaseService banco, SessaoService sessao, MidiaService midia)
    {
        _banco = banco;
        _sessao = sessao;
        _midia = midia;
    }

    /// <summary>Disparado quando um alerta vence, para as telas mostrarem o aviso visual.</summary>
    public event EventHandler<AlertaDisparadoEventArgs>? AlertaDisparado;

    public void Iniciar()
    {
        if (_relogio is not null)
            return;

        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
            return;

        _relogio = dispatcher.CreateTimer();
        _relogio.Interval = TimeSpan.FromSeconds(30);
        _relogio.Tick += async (_, _) => await VerificarAsync();
        _relogio.Start();
    }

    public void Parar()
    {
        _relogio?.Stop();
        _relogio = null;
    }

    public async Task VerificarAsync()
    {
        if (_sessao.CriancaAtual is null)
            return;

        var agora = DateTime.Now;
        var horaAtual = agora.ToString("HH:mm");

        foreach (var alerta in await _banco.ListarAlertasAtivosAsync(_sessao.CriancaAtual.IdCrianca))
        {
            if (alerta.Horario != horaAtual)
                continue;

            if (alerta.UltimoDisparo?.Date == agora.Date)
                continue;

            alerta.UltimoDisparo = agora;
            await _banco.SalvarAlertaAsync(alerta);

            var rotina = await _banco.ObterRotinaAsync(alerta.IdRotina);
            var mensagem = string.IsNullOrWhiteSpace(alerta.Mensagem)
                ? $"Daqui a pouco começa: {rotina?.Titulo}"
                : alerta.Mensagem;

            _midia.Vibrar(200);
            await _midia.FalarAsync(mensagem);

            AlertaDisparado?.Invoke(this, new AlertaDisparadoEventArgs(alerta, rotina, mensagem));
        }
    }

    /// <summary>Calcula o horário do alerta a partir do horário da rotina.</summary>
    public static string CalcularHorario(string horarioRotina, int antecedenciaMinutos)
    {
        if (!TimeSpan.TryParse(horarioRotina, out var hora))
            return horarioRotina;

        var alvo = hora - TimeSpan.FromMinutes(antecedenciaMinutos);
        if (alvo < TimeSpan.Zero)
            alvo += TimeSpan.FromDays(1);

        return alvo.ToString(@"hh\:mm");
    }
}

public class AlertaDisparadoEventArgs : EventArgs
{
    public AlertaDisparadoEventArgs(Alerta alerta, Rotina? rotina, string mensagem)
    {
        Alerta = alerta;
        Rotina = rotina;
        Mensagem = mensagem;
    }

    public Alerta Alerta { get; }
    public Rotina? Rotina { get; }
    public string Mensagem { get; }
}
