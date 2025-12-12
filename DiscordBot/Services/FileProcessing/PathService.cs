namespace DiscordBot.Services.FileProcessing;

public class PathService
{
    private readonly string _dataRoot;

    public static string SOUNDBOARD_SUBFOLDER = "soundboard";

    public PathService()
    {
        _dataRoot = Path.Combine(AppContext.BaseDirectory, "data");
        Directory.CreateDirectory(_dataRoot);
    }

    public string DataRoot => _dataRoot;

    public string DatabasePath => Path.Combine(_dataRoot, "bot.db");

    public string GetGuildFolder(ulong guildId, string? subfolder = null)
    {
        var path = Path.Combine(_dataRoot, "guilds", guildId.ToString());
        if (subfolder != null)
        {
            path = Path.Combine(path, subfolder);
        }
        Directory.CreateDirectory(path);
        return path;
    }
}
