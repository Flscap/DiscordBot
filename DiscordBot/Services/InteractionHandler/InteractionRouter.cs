using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Collections.Concurrent;

namespace DiscordBot.Services.InteractionHandler;

public class InteractionRouter
{
    public delegate Task InteractionHandler(SocketInteractionContext context, SocketMessageComponent component);

    private readonly ConcurrentDictionary<string, InteractionHandler> _interactionHandlers = new();
    private readonly ConcurrentDictionary<ulong, List<string>> _messageComponents = new();

    public void RegisterInteraction(string customId, InteractionHandler handler)
    {
        _interactionHandlers.TryAdd(customId, handler);
    }

    public async Task InvokeAsync(SocketInteractionContext context, SocketMessageComponent component)
    {
        if (_interactionHandlers.TryGetValue(component.Data.CustomId, out var handler))
        {
            try
            {
                await handler(context, component);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
            lock (customIds)
            {
                foreach (var customId in customIds)
                    _interactionHandlers.TryRemove(customId, out _);
            }
            _messageComponents.TryRemove(messageId, out _);
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
