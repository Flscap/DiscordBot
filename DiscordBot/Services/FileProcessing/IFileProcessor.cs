namespace DiscordBot.Services.FileProcessing;

public interface IFileProcessor
{
    Task<byte[]> ProcessAsync(byte[] data);
}
