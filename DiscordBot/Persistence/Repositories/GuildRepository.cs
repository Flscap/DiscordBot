using Dapper;
using DiscordBot.Persistence.Poco;

namespace DiscordBot.Persistence.Repositories;

public class GuildRepository : BaseRepository, IRepository<Guild>
{
    public GuildRepository(IDbService dbService) : base(dbService) { }

    public Task<Guild?> RetrieveByIdAsync(ulong id)
    {
        return _db.QuerySingleOrDefaultAsync<Guild>(
            "SELECT * FROM Guilds WHERE Id = @Id",
            new { Id = id }
        );
    }

    public Task<int> CreateAsync(Guild guild)
    {
        return _db.ExecuteAsync(
            "INSERT INTO Guilds (Id, SoundboardTextChannelId) VALUES (@Id, @SoundboardTextChannelId);",
            guild
        );
    }

    public Task<ulong> PersistAsync(Guild guild)
    {
        return PersistAsync(
            "INSERT INTO Guilds (Id, SoundboardTextChannelId) VALUES (@Id, @SoundboardTextChannelId);",
            guild
        );
    }

    public Task<int> UpdateAsync(Guild guild)
    {
        return _db.ExecuteAsync(
            "UPDATE Guilds SET SoundboardTextChannelId = @SoundboardTextChannelId WHERE Id = @Id;",
            guild
        );
    }

    public Task<int> DeleteAsync(ulong id)
    {
        return _db.ExecuteAsync(
            "DELETE FROM Guilds WHERE Id = @Id;",
            new { Id = id }
        );
    }
}
