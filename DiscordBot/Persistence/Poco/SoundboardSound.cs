namespace DiscordBot.Persistence.Poco;

public class SoundboardSound
{
    public ulong Id { get; set; }
    public string Label { get; set; } = null!;
    public string? Emoji { get; set; }
    public int ButtonStyle { get; set; }
    public string FilePath { get; set; } = null!;
    public ulong GuildId { get; set; }
}
