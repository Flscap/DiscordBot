
namespace DiscordBot.Services.FileProcessing.Processors;

public class SoundboardFileProcessor : IFileProcessor
{
    public Task<byte[]> ProcessAsync(byte[] data)
    {
        // Logic implemented yet,just example
        return Task.FromResult(data);
    }
}
