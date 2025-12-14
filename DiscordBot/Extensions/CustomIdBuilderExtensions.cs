using Discord;
using DiscordBot.Services.InteractionHandler;
using static DiscordBot.Services.InteractionHandler.InteractionRouter;

namespace DiscordBot.Extensions;

public enum InteractionNamespace {
    Music = 0,
    Soundboard = 1
}

public static class CustomIdBuilderExtensions
{
    private static InteractionRouter _interactionRouter = null!;

    public static void Configure(InteractionRouter interactionRouter)
    {
        _interactionRouter = interactionRouter;
    }

    public static ButtonBuilder WithInteractableCustomId(
        this ButtonBuilder builder,
        ulong guildId,
        InteractionNamespace interactionNamespace,
        InteractionHandler handler)
    {
        Register(builder, guildId, interactionNamespace, ComponentType.Button, handler);
        return builder;
    }

    private static void Register(
        IInteractableComponentBuilder builder,
        ulong guildId,
        InteractionNamespace interactionNamespace,
        ComponentType type,
        InteractionHandler handler)
    {
        string id = $"{guildId}:{type}:{interactionNamespace}:{Guid.NewGuid()}";
        builder.CustomId = id;
        _interactionRouter.RegisterInteraction(id, handler);
    }
}
