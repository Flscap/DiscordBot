
using Discord;
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
    private readonly InteractionRouter _interactionRouter;
    private readonly ILoggingService _loggingService;

    public InteractionHandlerService(DiscordSocketClient client, InteractionService interactionService, IServiceProvider serviceProvider, InteractionRouter interactionRouter, ILoggingService loggingService)
    {
        _client = client;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _interactionRouter = interactionRouter;
        _loggingService = loggingService;
    }

    public async Task InitializeAsync()
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        _client.InteractionCreated += HandleInteraction;
        _client.MessageDeleted += MessageDeleted;

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

        if (interaction is SocketMessageComponent component)
        {
            await _interactionRouter.InvokeAsync(ctx, component);
            return;
        }

        await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
    }

    private Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        _interactionRouter.UnregisterMessageComponents(message.Id);
        return Task.CompletedTask;
    }
}
