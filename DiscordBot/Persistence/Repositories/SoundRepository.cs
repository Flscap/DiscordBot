using Dapper;
using DiscordBot.Persistence.Poco;

namespace DiscordBot.Persistence.Repositories;

public class SoundRepository : IRepository<Sound>
{
    // ulong Id, string Label, string Emoji, int ButtonStyle
    private readonly IDbService _db;

    public SoundRepository(IDbService db)
    {
        _db = db;
    }

    public async Task<Sound?> GetByIdAsync(ulong id)
    {
        return await _db.Connection.QuerySingleOrDefaultAsync<Sound>(
            "SELECT * FROM Sounds WHERE Id = @Id",
            new
            {
                Id = id
            }
        );
    }

    public async Task<IEnumerable<Sound>> GetAllAsync()
    {
        return await _db.Connection.QueryAsync<Sound>("SELECT * FROM Sounds;");
    }

    public async Task<int> AddAsync(Sound sound)
    {
        return await _db.Connection.ExecuteAsync(
            "INSERT INTO Sounds (Label, Emoji, ButtonStyle, GuildId) VALUES (@Label, @Emoji, @ButtonStyle, @GuildId);",
            new
            {
                Label = sound.Label,
                Emoji = sound.Emoji,
                ButtonStyle = sound.ButtonStyle,
                GuildId = sound.GuildId
            }
        );
    }

    public async Task<int> UpdateAsync(Sound sound)
    {
        return await _db.Connection.ExecuteAsync(
            "UPDATE Sounds SET Label = @Label, Emoji = @Emoji, ButtonStyle = @ButtonStyle, GuildId = @GuildId WHERE Id = @Id;",
            new
            {
                Id = sound.Id,
                Label = sound.Label,
                Emoji = sound.Emoji,
                ButtonStyle = sound.ButtonStyle,
                GuildId = sound.GuildId
            }
        );
    }

    public async Task<int> DeleteAsync(ulong id)
    {
        return await _db.Connection.ExecuteAsync(
            "DELETE FROM Sounds WHERE Id = @Id;",
            new
            {
                Id = id
            }
        );
    }
}
