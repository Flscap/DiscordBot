using Dapper;
using DiscordBot.Persistence.Poco;

namespace DiscordBot.Persistence.Repositories;

public class SoundboardSoundRepository : BaseRepository, IRepository<SoundboardSound>
{
    public SoundboardSoundRepository(IDbService dbService) : base(dbService) { }

    public Task<SoundboardSound?> RetrieveByIdAsync(ulong id)
    {
        return _db.QuerySingleOrDefaultAsync<SoundboardSound>(
            "SELECT * FROM SoundboardSounds WHERE Id = @Id",
            new { Id = id }
        );
    }

    public Task<int> CreateAsync(SoundboardSound sound)
    {
        return _db.ExecuteAsync(
            "INSERT INTO SoundboardSounds (Label, Emoji, ButtonStyle, Path, GuildId) VALUES (@Label, @Emoji, @ButtonStyle, @Path, @GuildId);",
            sound
        );
    }

    public Task<ulong> PersistAsync(SoundboardSound sound)
    {
        return PersistAsync(
            "INSERT INTO SoundboardSounds (Label, Emoji, ButtonStyle, Path, GuildId) VALUES (@Label, @Emoji, @ButtonStyle, @Path, @GuildId);",
            sound
        );
    }

    public Task<int> UpdateAsync(SoundboardSound sound)
    {
        return _db.ExecuteAsync(
            "UPDATE SoundboardSounds SET Label = @Label, Emoji = @Emoji, ButtonStyle = @ButtonStyle, Path = @Path, GuildId = @GuildId WHERE Id = @Id;",
            sound
        );
    }

    public Task<int> DeleteAsync(ulong id)
    {
        return _db.ExecuteAsync(
            "DELETE FROM SoundboardSounds WHERE Id = @Id;",
            new { Id = id }
        );
    }
}
