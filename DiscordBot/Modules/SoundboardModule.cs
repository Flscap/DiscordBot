using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Persistence.Poco;
using DiscordBot.Persistence.Repositories;
using DiscordBot.Services.Logging;

namespace DiscordBot.Modules;

[Group("soundboard", "Soundboard related commands")]
public class SoundboardModule : InteractionModuleBase<SocketInteractionContext>
{

    private static readonly string FFMPEG_ARGS_REMOTE = "-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -vn";

    private readonly ILoggingService _logger;

    public SoundboardModule(ILoggingService loggingService)
    {
        _logger = loggingService;
    }

    [SlashCommand("add", "Add sound to Soundboard")]
    public async Task AddSound(
        [Summary(description: "Label to be shown on the button")] string label,
        [Summary(description: "Emoji to be shown to the left of the label")] string emoji)
    {
        await FollowupAsync($"🎶 Playing");
    }

    [Group("channel", "Soundboard channel related commands")]
    public class SoundboardChannelModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILoggingService _logger;
        private GuildRepository _guildRepository;

        public SoundboardChannelModule(ILoggingService loggingService, GuildRepository guildRepository)
        {
            _logger = loggingService;
            _guildRepository = guildRepository;
        }

        [SlashCommand("get", "Get Soundboard channel")]
        public async Task GetSoundboardChannelAsync()
        {
            Guild? guild = await _guildRepository.GetByIdAsync(Context.Guild.Id);

            if (guild == null)
            {
                await RespondAsync($"Soundboard channel is not set", ephemeral: true);
            }
            else
            {
                SocketTextChannel soundboardTextChannel = Context.Guild.GetTextChannel(guild.SoundboardTextChannelId);
                await RespondAsync($"The Soundboard channel is: {soundboardTextChannel.Name}", ephemeral: true);
            }
        }

        [SlashCommand("set", "Set Soundboard channel")]
        public async Task StopAsync(
            [Summary(description: "Id of the Channel to be set as dedicated Soundboard channel")] string channelId)
        {
            if (ulong.TryParse(channelId, out ulong channelIdUlong))
            {
                SocketTextChannel textChannel = Context.Guild.GetTextChannel(channelIdUlong);
                if (textChannel != null)
                {
                    Guild? guild = await _guildRepository.GetByIdAsync(Context.Guild.Id);
                    if (guild != null)
                    {
                        if(guild.SoundboardTextChannelId != channelIdUlong)
                        {
                            guild.SoundboardTextChannelId = channelIdUlong;
                            await _guildRepository.UpdateAsync(guild);
                        }
                        else
                        {
                            await RespondAsync($"Soundboard channel is already set to: {textChannel.Name}", ephemeral: true);
                        }
                    }
                    else
                    {
                        Guild newGuild = new Guild
                        {
                            Id = Context.Guild.Id,
                            SoundboardTextChannelId = channelIdUlong
                        };
                        await _guildRepository.AddAsync(newGuild);
                    }
                    await RespondAsync($"Soundboard channel set to: {textChannel.Name}", ephemeral: true);
                }
                else
                {
                    await RespondAsync($"Channel with Id: {channelId} doesn't exist", ephemeral: true);
                }
            }
            else
            {
                await RespondAsync($"Invalid channel Id: {channelId}", ephemeral: true);
            }
        }
    }
}