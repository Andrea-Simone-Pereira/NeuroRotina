using SQLite;
using NeuroRotina.Models;

namespace NeuroRotina.Data;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        string caminhoBanco = Path.Combine(
            FileSystem.AppDataDirectory,
            "neurorotina.db3");

        _database = new SQLiteAsyncConnection(caminhoBanco);
    }

    public async Task InicializarAsync()
    {
        await _database.CreateTableAsync<Usuario>();
        await _database.CreateTableAsync<Crianca>();
        await _database.CreateTableAsync<Rotina>();
        await _database.CreateTableAsync<Atividade>();

        await AtualizarBancoAsync();
    }

    private async Task AtualizarBancoAsync()
    {
        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Crianca ADD COLUMN NivelSuporte TEXT");
        }
        catch
        {
            // A coluna já existe.
        }

        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Crianca ADD COLUMN Pin TEXT");
        }
        catch
        {
            // A coluna já existe.
        }

        try
        {
            await _database.ExecuteAsync(
                "ALTER TABLE Atividade ADD COLUMN Horario TEXT");
        }
        catch
        {
            // A coluna já existe.
        }
    }

    public async Task<int> CadastrarUsuarioAsync(Usuario usuario)
    {
        return await _database.InsertAsync(usuario);
    }

    public async Task<Usuario?> BuscarUsuarioAsync(
        string email,
        string senha)
    {
        return await _database
            .Table<Usuario>()
            .Where(u => u.Email == email && u.Senha == senha)
            .FirstOrDefaultAsync();
    }

    public async Task<Usuario?> BuscarUsuarioPorEmailAsync(
        string email)
    {
        return await _database
            .Table<Usuario>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CadastrarCriancaAsync(
        Crianca crianca)
    {
        return await _database.InsertAsync(crianca);
    }

    public async Task<List<Crianca>> BuscarCriancasAsync()
    {
        return await _database
            .Table<Crianca>()
            .ToListAsync();
    }

    public async Task<List<Crianca>> BuscarCriancasPorUsuarioAsync(
        int usuarioId)
    {
        return await _database
            .Table<Crianca>()
            .Where(c => c.UsuarioId == usuarioId)
            .ToListAsync();
    }

    public async Task<Crianca?> BuscarCriancaPorNomeEPinAsync(
        string nome,
        string pin)
    {
        string nomeBusca = nome.Trim().ToUpper();

        List<Crianca> criancas =
            await _database
                .Table<Crianca>()
                .ToListAsync();

        return criancas.FirstOrDefault(c =>
            c.Nome.Trim().ToUpper() == nomeBusca &&
            c.Pin == pin);
    }

    public async Task<int> CadastrarRotinaAsync(
        Rotina rotina)
    {
        return await _database.InsertAsync(rotina);
    }

    public async Task<List<Rotina>> BuscarRotinasPorCriancaAsync(
        int criancaId)
    {
        return await _database
            .Table<Rotina>()
            .Where(r => r.CriancaId == criancaId)
            .ToListAsync();
    }

    public async Task<int> AtualizarRotinaAsync(
        Rotina rotina)
    {
        return await _database.UpdateAsync(rotina);
    }

    public async Task<int> CadastrarAtividadeAsync(
        Atividade atividade)
    {
        return await _database.InsertAsync(atividade);
    }

    public async Task<List<Atividade>> BuscarAtividadesPorRotinaAsync(
        int rotinaId)
    {
        return await _database
            .Table<Atividade>()
            .Where(a => a.RotinaId == rotinaId)
            .ToListAsync();
    }

    public async Task<int> AtualizarAtividadeAsync(
        Atividade atividade)
    {
        return await _database.UpdateAsync(atividade);
    }
}