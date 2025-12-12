namespace DiscordBot.Components.Buttons;

public class SoundboardButton : CustomButtonBase
{
    public ulong SoundboardSoundId { get; set; }
    public string FilePath { get; set; } = null!;

    public override string BuildCustomId() => SoundboardSoundId.ToString();
}
