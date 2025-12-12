namespace DiscordBot.Services.FileProcessing;

public interface IFileProcessingService
{
    Task<string> DownloadAndSaveAttachmentAsync(string url, ulong guildId, IFileProcessor? fileProcessor);
}
