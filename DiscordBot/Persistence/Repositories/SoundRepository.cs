using Dapper;
using DiscordBot.Persistence.Poco;

namespace DiscordBot.Persistence.Repositories;

public class SoundRepository : BaseRepository, IRepository<Sound>
{
    public SoundRepository(IDbService dbService) : base(dbService) { }

    public Task<Sound?> RetrieveByIdAsync(ulong id)
    {
        return _db.QuerySingleOrDefaultAsync<Sound>(
            "SELECT * FROM Sounds WHERE Id = @Id",
            new { Id = id }
        );
    }

    public Task<int> CreateAsync(Sound sound)
    {
        return _db.ExecuteAsync(
            "INSERT INTO Sounds (Label, Emoji, ButtonStyle, GuildId) VALUES (@Label, @Emoji, @ButtonStyle, @GuildId);",
            sound
        );
    }

    public Task<ulong> PersistAsync(Sound sound)
    {
        return PersistAsync(
            "INSERT INTO Sounds (Label, Emoji, ButtonStyle, GuildId) VALUES (@Label, @Emoji, @ButtonStyle, @GuildId);",
            sound
        );
    }

    public Task<int> UpdateAsync(Sound sound)
    {
        return _db.ExecuteAsync(
            "UPDATE Sounds SET Label = @Label, Emoji = @Emoji, ButtonStyle = @ButtonStyle, GuildId = @GuildId WHERE Id = @Id;",
            sound
        );
    }

    public Task<int> DeleteAsync(ulong id)
    {
        return _db.ExecuteAsync(
            "DELETE FROM Sounds WHERE Id = @Id;",
            new { Id = id }
        );
    }
}
