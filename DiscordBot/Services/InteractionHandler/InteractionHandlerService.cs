
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Services.Logging;
using System.Reflection;

namespace DiscordBot.Services.InteractionHandler;

public class InteractionHandlerService : IInteractionHandlerService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggingService _loggingService;

    public InteractionHandlerService(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider, ILoggingService loggingService)
    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _loggingService = loggingService;
    }

    public async Task InitializeAsync()
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        _client.InteractionCreated += HandleInteraction;

        _client.Ready += async () =>
        {
            try
            {
                await _interactionService.RegisterCommandsGloballyAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        };
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(_client, interaction);

        await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
    }
}
