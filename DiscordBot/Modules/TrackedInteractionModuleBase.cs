using Discord;
using Discord.Interactions;
using DiscordBot.Services.InteractionHandler;

namespace DiscordBot.Modules;

public abstract class TrackedInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    protected readonly InteractionRouter _interactionRouter;

    protected TrackedInteractionModuleBase(InteractionRouter interactionRouter)
    {
        _interactionRouter = interactionRouter;
    }

    protected async Task<IUserMessage?> RespondAndStoreAsync(string text = null, Embed[] embeds = null, bool isTTS = false, bool ephemeral = false, AllowedMentions allowedMentions = null, RequestOptions options = null, MessageComponent components = null, Embed embed = null, PollProperties poll = null, MessageFlags flags = MessageFlags.None)
    {
        await RespondAsync(text, embeds, isTTS, ephemeral, allowedMentions, options, components, embed, poll, flags);
        var message = await Context.Interaction.GetOriginalResponseAsync() as IUserMessage;
        
        if (message == null || message.Components.Count == 0)
            return message;
        
        List<string> customIds = new List<string>();
        foreach (var row in message.Components.OfType<ActionRowComponent>())
        {
            foreach (var component in row.Components.OfType<IInteractableComponent>())
            {
                var customId = component.CustomId;
                customIds.Add(customId);
            }
        }

        if (customIds.Count > 0)
            _interactionRouter.RegisterMessageComponents(message.Id, customIds);

        return message;
    }
}
