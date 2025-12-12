namespace DiscordBot.Services.FileProcessing;

public interface IFileProcessingService
{
    Task<string> DownloadAndSaveAttachmentAsync(string url, string extension, ulong guildId, string subfolder, IFileProcessor? fileProcessor);
}
