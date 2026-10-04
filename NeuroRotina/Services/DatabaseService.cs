using NeuroRotina.Models;
using SQLite;

namespace NeuroRotina.Services;

/// <summary>
/// Camada de persistência do aplicativo. Usa SQLite local, sem servidor,
/// o que atende à restrição do projeto de não haver orçamento para infraestrutura paga.
/// </summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _conexao;

    public static string CaminhoBanco =>
        Path.Combine(FileSystem.AppDataDirectory, "neurorotina.db3");

    private async Task<SQLiteAsyncConnection> ObterConexaoAsync()
    {
        if (_conexao is not null)
            return _conexao;

        _conexao = new SQLiteAsyncConnection(
            CaminhoBanco,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await _conexao.CreateTablesAsync(
            CreateFlags.None,
            typeof(Usuario), typeof(Crianca), typeof(Rotina),
            typeof(Atividade), typeof(Alerta), typeof(Historico), typeof(Cartao));

        return _conexao;
    }

    // ---------------------------------------------------------------- Usuário

    public async Task<bool> EmailJaCadastradoAsync(string email)
    {
        var db = await ObterConexaoAsync();
        var emailNormalizado = email.Trim().ToLowerInvariant();
        var achado = await db.Table<Usuario>()
            .Where(u => u.Email == emailNormalizado)
            .FirstOrDefaultAsync();
        return achado is not null;
    }

    public async Task<int> SalvarUsuarioAsync(Usuario usuario)
    {
        var db = await ObterConexaoAsync();
        usuario.Email = usuario.Email.Trim().ToLowerInvariant();

        if (usuario.IdUsuario == 0)
        {
            await db.InsertAsync(usuario);
            return usuario.IdUsuario;
        }

        await db.UpdateAsync(usuario);
        return usuario.IdUsuario;
    }

    /// <summary>Método login() do diagrama de classes.</summary>
    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var db = await ObterConexaoAsync();
        var emailNormalizado = email.Trim().ToLowerInvariant();
        var usuario = await db.Table<Usuario>()
            .Where(u => u.Email == emailNormalizado)
            .FirstOrDefaultAsync();

        if (usuario is null)
            return null;

        return SenhaHelper.Conferir(senha, usuario.Senha) ? usuario : null;
    }

    // ---------------------------------------------------------------- Criança

    public async Task<List<Crianca>> ListarCriancasAsync(int idCuidador)
    {
        var db = await ObterConexaoAsync();
        return await db.Table<Crianca>()
            .Where(c => c.IdCuidador == idCuidador)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Crianca?> ObterCriancaAsync(int idCrianca)
    {
        var db = await ObterConexaoAsync();
        return await db.Table<Crianca>()
            .Where(c => c.IdCrianca == idCrianca)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarCriancaAsync(Crianca crianca)
    {
        var db = await ObterConexaoAsync();

        if (crianca.IdCrianca == 0)
        {
            await db.InsertAsync(crianca);
            await CriarCartoesPadraoAsync(crianca.IdCrianca);
            return crianca.IdCrianca;
        }

        await db.UpdateAsync(crianca);
        return crianca.IdCrianca;
    }

    public async Task ExcluirCriancaAsync(int idCrianca)
    {
        var db = await ObterConexaoAsync();

        foreach (var rotina in await db.Table<Rotina>().Where(r => r.IdCrianca == idCrianca).ToListAsync())
            await ExcluirRotinaAsync(rotina.IdRotina);

        await db.Table<Cartao>().Where(c => c.IdCrianca == idCrianca).DeleteAsync();
        await db.Table<Historico>().Where(h => h.IdCrianca == idCrianca).DeleteAsync();
        await db.DeleteAsync<Crianca>(idCrianca);
    }

    // ---------------------------------------------------------------- Rotina

    public async Task<List<Rotina>> ListarRotinasAsync(int idCrianca)
    {
        var db = await ObterConexaoAsync();
        var rotinas = await db.Table<Rotina>()
            .Where(r => r.IdCrianca == idCrianca)
            .ToListAsync();

        foreach (var rotina in rotinas)
            rotina.TotalAtividades = await db.Table<Atividade>()
                .Where(a => a.IdRotina == rotina.IdRotina)
                .CountAsync();

        return rotinas.OrderBy(r => r.Horario).ToList();
    }

    public async Task<Rotina?> ObterRotinaAsync(int idRotina)
    {
        var db = await ObterConexaoAsync();
        return await db.Table<Rotina>()
            .Where(r => r.IdRotina == idRotina)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarRotinaAsync(Rotina rotina)
    {
        var db = await ObterConexaoAsync();

        if (rotina.IdRotina == 0)
        {
            await db.InsertAsync(rotina);
            return rotina.IdRotina;
        }

        await db.UpdateAsync(rotina);
        return rotina.IdRotina;
    }

    public async Task ExcluirRotinaAsync(int idRotina)
    {
        var db = await ObterConexaoAsync();
        await db.Table<Atividade>().Where(a => a.IdRotina == idRotina).DeleteAsync();
        await db.Table<Alerta>().Where(a => a.IdRotina == idRotina).DeleteAsync();
        await db.DeleteAsync<Rotina>(idRotina);
    }

    // ---------------------------------------------------------------- Atividade

    public async Task<List<Atividade>> ListarAtividadesAsync(int idRotina)
    {
        var db = await ObterConexaoAsync();
        return await db.Table<Atividade>()
            .Where(a => a.IdRotina == idRotina)
            .OrderBy(a => a.Ordem)
            .ToListAsync();
    }

    public async Task<Atividade?> ObterAtividadeAsync(int idAtividade)
    {
        var db = await ObterConexaoAsync();
        return await db.Table<Atividade>()
            .Where(a => a.IdAtividade == idAtividade)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SalvarAtividadeAsync(Atividade atividade)
    {
        var db = await ObterConexaoAsync();

        if (atividade.IdAtividade == 0)
        {
            atividade.Ordem = await db.Table<Atividade>()
                .Where(a => a.IdRotina == atividade.IdRotina)
                .CountAsync();
            await db.InsertAsync(atividade);
            return atividade.IdAtividade;
        }

        await db.UpdateAsync(atividade);
        return atividade.IdAtividade;
    }

    public async Task ExcluirAtividadeAsync(int idAtividade)
    {
        var db = await ObterConexaoAsync();
        await db.DeleteAsync<Atividade>(idAtividade);
    }

    public async Task ReiniciarAtividadesAsync(int idRotina)
    {
        var db = await ObterConexaoAsync();
        foreach (var atividade in await ListarAtividadesAsync(idRotina))
        {
            atividade.Concluida = false;
            await db.UpdateAsync(atividade);
        }
    }

    // ---------------------------------------------------------------- Alerta

    public async Task<Alerta?> ObterAlertaDaRotinaAsync(int idRotina)
    {
        var db = await ObterConexaoAsync();
        return await db.Table<Alerta>()
            .Where(a => a.IdRotina == idRotina)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Alerta>> ListarAlertasAtivosAsync(int idCrianca)
    {
        var db = await ObterConexaoAsync();
        var rotinas = await db.Table<Rotina>()
            .Where(r => r.IdCrianca == idCrianca && r.Ativa)
            .ToListAsync();

        var ids = rotinas.Select(r => r.IdRotina).ToList();
        var alertas = await db.Table<Alerta>().Where(a => a.Ativo).ToListAsync();

        return alertas.Where(a => ids.Contains(a.IdRotina)).ToList();
    }

    public async Task SalvarAlertaAsync(Alerta alerta)
    {
        var db = await ObterConexaoAsync();

        if (alerta.IdAlerta == 0)
            await db.InsertAsync(alerta);
        else
            await db.UpdateAsync(alerta);
    }

    // ---------------------------------------------------------------- Histórico

    public async Task<List<Historico>> ListarHistoricoAsync(int idCrianca)
    {
        var db = await ObterConexaoAsync();
        var registros = await db.Table<Historico>()
            .Where(h => h.IdCrianca == idCrianca)
            .ToListAsync();

        return registros.OrderByDescending(h => h.Data).ToList();
    }

    public async Task RegistrarComportamentoAsync(Historico historico)
    {
        var db = await ObterConexaoAsync();
        await db.InsertAsync(historico);
    }

    public async Task ExcluirHistoricoAsync(int idHistorico)
    {
        var db = await ObterConexaoAsync();
        await db.DeleteAsync<Historico>(idHistorico);
    }

    // ---------------------------------------------------------------- Cartão

    public async Task<List<Cartao>> ListarCartoesAsync(int idCrianca)
    {
        var db = await ObterConexaoAsync();
        return await db.Table<Cartao>()
            .Where(c => c.IdCrianca == idCrianca)
            .OrderBy(c => c.Ordem)
            .ToListAsync();
    }

    public async Task<Cartao?> ObterCartaoAsync(int idCartao)
    {
        var db = await ObterConexaoAsync();
        return await db.Table<Cartao>()
            .Where(c => c.IdCartao == idCartao)
            .FirstOrDefaultAsync();
    }

    public async Task SalvarCartaoAsync(Cartao cartao)
    {
        var db = await ObterConexaoAsync();

        if (cartao.IdCartao == 0)
        {
            cartao.Ordem = await db.Table<Cartao>()
                .Where(c => c.IdCrianca == cartao.IdCrianca)
                .CountAsync();
            await db.InsertAsync(cartao);
            return;
        }

        await db.UpdateAsync(cartao);
    }

    public async Task ExcluirCartaoAsync(int idCartao)
    {
        var db = await ObterConexaoAsync();
        await db.DeleteAsync<Cartao>(idCartao);
    }

    /// <summary>
    /// Toda criança começa com uma prancha mínima de comunicação, para que o
    /// aplicativo já seja útil antes de qualquer configuração do cuidador.
    /// </summary>
    private async Task CriarCartoesPadraoAsync(int idCrianca)
    {
        var db = await ObterConexaoAsync();

        var padrao = new (string Texto, string Icone, string Categoria)[]
        {
            ("Sim", "✅", "Respostas"),
            ("Não", "❌", "Respostas"),
            ("Quero água", "💧", "Necessidades"),
            ("Estou com fome", "🍎", "Necessidades"),
            ("Banheiro", "🚻", "Necessidades"),
            ("Estou com dor", "🤕", "Sentimentos"),
            ("Estou cansado", "😴", "Sentimentos"),
            ("Preciso de ajuda", "🙋", "Necessidades"),
            ("Quero brincar", "🧸", "Atividades"),
            ("Quero música", "🎵", "Atividades"),
            ("Muito barulho", "🔊", "Sentimentos"),
            ("Quero um abraço", "🤗", "Sentimentos")
        };

        var ordem = 0;
        foreach (var (texto, icone, categoria) in padrao)
        {
            await db.InsertAsync(new Cartao
            {
                IdCrianca = idCrianca,
                Texto = texto,
                Icone = icone,
                Categoria = categoria,
                Ordem = ordem++
            });
        }
    }
}
