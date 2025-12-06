using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace DiscordBot.Services.CommandHandler;

public class TextCommandHandlerService : ITextCommandHandlerService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commandService;
    private readonly IServiceProvider _serviceProvider;

    public TextCommandHandlerService(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider)
    {
        _client = client;
        _commandService = commandService;
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        _client.MessageReceived += MessageReceived;

        await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
    }

    private async Task MessageReceived(SocketMessage message)
    {
        if (message is not SocketUserMessage msg) return;
        Console.WriteLine($"message: {message}");

        int argPos = 0;
        if (!msg.HasCharPrefix('!', ref argPos)) return;
        Console.WriteLine("start with !");

        var context = new SocketCommandContext(_client, msg);

        await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
    }
}
