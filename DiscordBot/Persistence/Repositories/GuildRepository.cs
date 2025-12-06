using Dapper;
using DiscordBot.Persistence.Poco;

namespace DiscordBot.Persistence.Repositories;

public class GuildRepository : IRepository<Guild>
{
    private readonly IDbService _db;

    public GuildRepository(IDbService db)
    {
        _db = db;
    }

    public async Task<Guild?> GetByIdAsync(ulong id)
    {
        return await _db.Connection.QuerySingleOrDefaultAsync<Guild>(
            "SELECT * FROM Guilds WHERE Id = @Id",
            new
            {
                Id = id
            }
        );
    }

    public async Task<IEnumerable<Guild>> GetAllAsync()
    {
        return await _db.Connection.QueryAsync<Guild>("SELECT * FROM Guilds;");
    }

    public async Task<int> AddAsync(Guild guild)
    {
        return await _db.Connection.ExecuteAsync(
            "INSERT INTO Guilds (Id, SoundboardTextChannelId) VALUES (@Id, @SoundboardTextChannelId);",
            new
            {
                Id = guild.Id,
                SoundboardTextChannelId = guild.SoundboardTextChannelId
            }
        );
    }

    public async Task<int> UpdateAsync(Guild guild)
    {
        return await _db.Connection.ExecuteAsync(
            "UPDATE Guilds SET SoundboardTextChannelId = @SoundboardTextChannelId WHERE Id = @Id;",
            new
            {
                Id = guild.Id,
                SoundboardTextChannelId = guild.SoundboardTextChannelId
            }
        );
    }

    public async Task<int> DeleteAsync(ulong id)
    {
        return await _db.Connection.ExecuteAsync(
            "DELETE FROM Guilds WHERE Id = @Id;",
            new
            {
                Id = id
            }
        );
    } 
}
