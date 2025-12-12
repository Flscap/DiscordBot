using Discord;

namespace DiscordBot.Components.Buttons;

public abstract class CustomButtonBase
{
    public string Label { get; set; } = string.Empty;
    public string? Emoji { get; set; }
    public ButtonStyle Style { get; set; } = ButtonStyle.Primary;

    public abstract string BuildCustomId();

    public virtual ButtonBuilder ToDiscordButton()
    {
        return new ButtonBuilder()
            .WithCustomId(BuildCustomId())
            .WithLabel(Label)
            .WithEmote(Emoji != null ? new Emoji(Emoji) : null)
            .WithStyle(Style);
    }

}
