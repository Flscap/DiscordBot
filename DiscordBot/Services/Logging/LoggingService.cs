using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Services.Logging;

public class LoggingService : ILoggingService
{
    public void Initialize(IServiceProvider services)
    {
        var client = services.GetRequiredService<DiscordSocketClient>();
        var commandService = services.GetRequiredService<CommandService>();
        var interactionService = services.GetRequiredService<InteractionService>();

        client.Log += LogAsync;
        commandService.Log += LogCommandAsync;
        interactionService.Log += LogInteractionAsync;
    }

    public Task LogAsync(LogMessage message)
    {
        Console.WriteLine($"[{message.Severity}] {message.Source}: {message.Message}");
        return Task.CompletedTask;
    }

    public Task LogAsync(string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }

    public Task LogCommandAsync(LogMessage message)
    {
        Console.WriteLine($"[{message.Severity}] {message.Source}: {message.Message}");
        return Task.CompletedTask;
    }

    public Task LogInteractionAsync(LogMessage message)
    {
        Console.WriteLine($"[{message.Severity}] {message.Source}: {message.Message}");
        return Task.CompletedTask;
    }


}
