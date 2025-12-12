namespace DiscordBot.Services.FileProcessing;

public class PathService
{
    private readonly string _dataRoot;

    public PathService()
    {
        _dataRoot = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(_dataRoot);
    }

    public string DataRoot => _dataRoot;

    public string DatabasePath => Path.Combine(_dataRoot, "bot.db");

    public string GetGuildFolder(ulong guildId)
    {
        var path = Path.Combine(_dataRoot, "guilds", guildId.ToString());
        Directory.CreateDirectory(path);
        return path;
    }
}
