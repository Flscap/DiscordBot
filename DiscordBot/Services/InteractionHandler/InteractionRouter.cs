using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Services.InteractionHandler;

public class InteractionRouter
{
    public delegate Task InteractionHandler(SocketInteractionContext context, SocketMessageComponent component);

    private Dictionary<string, InteractionHandler> _interactionHandlers = null!;
    private Dictionary<ulong, List<string>> _messageComponents = null!;

    public InteractionRouter()
    {
        _interactionHandlers = new Dictionary<string, InteractionHandler>();
        _messageComponents = new Dictionary<ulong, List<string>>();
    }

    public void RegisterInteraction(string customId, InteractionHandler handler)
    {
        _interactionHandlers.TryAdd(customId, handler);
    }

    public async Task InvokeAsync(SocketInteractionContext context, SocketMessageComponent component)
    {
        if (_interactionHandlers.TryGetValue(component.Data.CustomId, out var handler))
        {
            await handler(context, component);
        }
    }

    public void RegisterMessageComponents(ulong messageId, List<string> customIds)
    {
        _messageComponents.TryAdd(messageId, customIds);
        Print();
    }

    public void UnregisterMessageComponents(ulong messageId)
    {
        if (_messageComponents.TryGetValue(messageId, out var customIds))
        {
            foreach (var customId in customIds)
            {
                _interactionHandlers.Remove(customId);
            }
            _messageComponents.Remove(messageId);
        }
        Print();
    }

    private void Print()
    {
        var messageComponents = string.Join(System.Environment.NewLine, _messageComponents.Select(messageComponent => $"{messageComponent.Key}:{System.Environment.NewLine}{string.Join(System.Environment.NewLine, messageComponent.Value)}"));
        var interactionHandlers = string.Join(System.Environment.NewLine, _interactionHandlers.Select(interactionHandler => $"{interactionHandler.Key}"));

        Console.WriteLine($"messageComponents:{System.Environment.NewLine}{messageComponents}");
        Console.WriteLine($"interactionHandlers:{System.Environment.NewLine}{interactionHandlers}");
    }
}
