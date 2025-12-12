namespace DiscordBot.Services.FileProcessing;

public class FileProcessingService : IFileProcessingService
{
    private readonly PathService _pathService;

    public FileProcessingService(PathService pathService)
    {
        _pathService = pathService;
    }

    public async Task<string> DownloadAndSaveAttachmentAsync(string url, string extension, ulong guildId, string subfolder, IFileProcessor? fileProcessor)
    {
        var data = await DownloadAsync(url);

        if (fileProcessor != null)
        {
            data = await fileProcessor.ProcessAsync(data);
        }

        var filePath = await SaveAsync(data, extension, guildId, subfolder);
        return filePath;
    }

    private async Task<byte[]> DownloadAsync(string url)
    {
        using var client = new HttpClient();
        return await client.GetByteArrayAsync(url);
    }

    private async Task<string> SaveAsync(byte[] data, string extension, ulong guildId, string? subfolder)
    {
        var guildFolder = _pathService.GetGuildFolder(guildId, subfolder);
        var filename = $"{Guid.NewGuid()}.{extension}";
        var path = Path.Combine(guildFolder, filename);

        await File.WriteAllBytesAsync(path, data);
        return path;
    }
}
