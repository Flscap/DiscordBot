using Discord;

namespace DiscordBot.Services.Logging;

public interface ILoggingService
{
    void Initialize(IServiceProvider serviceProvider);
    Task LogAsync(LogMessage message);
    Task LogAsync(string message);
    Task LogCommandAsync(LogMessage message);
    Task LogInteractionAsync(LogMessage message);
}
