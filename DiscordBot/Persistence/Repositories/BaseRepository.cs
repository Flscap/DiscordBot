using Dapper;
using System.Data;

namespace DiscordBot.Persistence.Repositories;

public abstract class BaseRepository
{
    protected readonly IDbService _dbService;
    protected IDbConnection _db => _dbService.Connection;

    protected BaseRepository(IDbService dbService)
    {
        _dbService = dbService;
    }

    protected async Task<ulong> PersistAsync(string sql, object param)
    {
        using var transaction = _db.BeginTransaction();

        try
        {
            await _db.ExecuteAsync(sql, param, transaction);

            ulong id = await _db.ExecuteScalarAsync<ulong>(
                "SELECT last_insert_rowid()",
                transaction: transaction
            );

            transaction.Commit();
            return id;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
