namespace DiscordBot.Persistence.Poco;

public class SoundboardSound
{
    public ulong Id { get; set; }
    public string? Label { get; set; }
    public string? Emoji { get; set; }
    public int ButtonStyle { get; set; }
    public string? Path { get; set; }
    public ulong GuildId { get; set; }
}
