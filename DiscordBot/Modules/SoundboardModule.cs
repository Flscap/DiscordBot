using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Components.Buttons;
using DiscordBot.Persistence.Poco;
using DiscordBot.Persistence.Repositories;
using DiscordBot.Services.FileProcessing;
using DiscordBot.Services.FileProcessing.Processors;
using DiscordBot.Services.Logging;
using System.Reflection.Emit;

namespace DiscordBot.Modules;

[Group("soundboard", "Soundboard related commands")]
public class SoundboardModule : InteractionModuleBase<SocketInteractionContext>
{
    public SoundboardModule()
    {

    }

    [Group("channel", "Soundboard channel related commands")]
    public partial class SoundboardChannelModule : InteractionModuleBase<SocketInteractionContext>
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
            Guild? guild = await _guildRepository.RetrieveByIdAsync(Context.Guild.Id);

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
                    Guild? guild = await _guildRepository.RetrieveByIdAsync(Context.Guild.Id);
                    if (guild != null)
                    {
                        if (guild.SoundboardTextChannelId != channelIdUlong)
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
                        await _guildRepository.CreateAsync(newGuild);
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

    [Group("sound", "Soundboard sound related commands")]
    public partial class SoundboardSoundModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ILoggingService _logger;
        private readonly SoundboardSoundRepository _soundRepository;
        private readonly IFileProcessingService _fileProcessingService;

        public SoundboardSoundModule(ILoggingService loggingService, SoundboardSoundRepository soundRepository, IFileProcessingService fileProcessingService)
        {
            _logger = loggingService;
            _soundRepository = soundRepository;
            _fileProcessingService = fileProcessingService;
        }

        [SlashCommand("add", "Add sound to Soundboard")]
        public async Task AddSound(
            [Summary(description: "Label to be shown on the button")] string label,
            [Summary(description: "Emoji to be shown to the left of the label")] string emoji)
        {
            // Step 1: Acknowledge command
            await RespondAsync($"You chose label `{label}` and emoji `{emoji}`. Now select a button style:", ephemeral: true);

            // Step 2: Build buttons for all ButtonStyle enum values
            var builder = new ComponentBuilder();
            foreach (ButtonStyle style in Enum.GetValues(typeof(ButtonStyle)))
            {
                if (style == ButtonStyle.Link || style == ButtonStyle.Premium)
                    continue;

                builder.WithButton(
                    label: label,
                    customId: $"style_{style}",
                    style: style,
                    emote: string.IsNullOrWhiteSpace(emoji) ? null : new Emoji(emoji)
                );
            }

            var message = await FollowupAsync("Select a button style:", components: builder.Build(), ephemeral: true);

            // Wait for the user to click one of the buttons
            var tcs = new TaskCompletionSource<SocketMessageComponent>();
            Task Handler(SocketMessageComponent component)
            {
                if (component.User.Id == Context.User.Id &&
                    component.Message.Id == message.Id)
                {
                    tcs.TrySetResult(component);
                }
                return Task.CompletedTask;
            }

            Context.Client.ButtonExecuted += Handler;

            var timeoutTask = Task.Delay(TimeSpan.FromMinutes(2));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            Context.Client.ButtonExecuted -= Handler;

            if (completedTask != tcs.Task)
            {
                await FollowupAsync("No style selected in time. Command cancelled.", ephemeral: true);
                return;
            }

            var selectedComponent = tcs.Task.Result;
            await selectedComponent.DeferAsync(); // Acknowledge button click

            // Extract the chosen ButtonStyle from the customId
            var chosenStyleStr = selectedComponent.Data.CustomId.Replace("style_", "");
            Enum.TryParse<ButtonStyle>(chosenStyleStr, out var chosenStyle);

            await FollowupAsync($"Now please upload your sound file.", ephemeral: true);

            // Step 3: Wait for file upload (same as your previous code)
            var userId = Context.User.Id;
            var channelId = Context.Channel.Id;

            TaskCompletionSource<IUserMessage> tcsMessage = new TaskCompletionSource<IUserMessage>();
            Task MessageHandler(SocketMessage msg)
            {
                if (msg.Channel.Id == channelId &&
                    msg.Author.Id == userId &&
                    msg is IUserMessage userMsg &&
                    userMsg.Attachments.Count > 0)
                {
                    tcsMessage.TrySetResult(userMsg);
                }
                return Task.CompletedTask;
            }

            Context.Client.MessageReceived += MessageHandler;

            var messageTimeout = Task.Delay(TimeSpan.FromMinutes(2));
            var completedMsgTask = await Task.WhenAny(tcsMessage.Task, messageTimeout);

            Context.Client.MessageReceived -= MessageHandler;

            if (completedMsgTask != tcsMessage.Task)
            {
                await FollowupAsync("No file uploaded in time. Command cancelled.", ephemeral: true);
                return;
            }

            var attachment = tcsMessage.Task.Result.Attachments.First();
            await FollowupAsync($"Received file: {attachment.Filename}. Label: {label}, Emoji: {emoji}, ButtonStyle: {chosenStyle}", ephemeral: true);

            SoundboardFileProcessor processor = new SoundboardFileProcessor();
            string filePath = await _fileProcessingService.DownloadAndSaveAttachmentAsync(attachment.Url, attachment.Filename.Split(".").LastOrDefault() ?? string.Empty, Context.Guild.Id, PathService.SOUNDBOARD_SUBFOLDER, processor);

            await _soundRepository.CreateAsync(new SoundboardSound
            {
                Label = label,
                Emoji = emoji,
                ButtonStyle = (int)chosenStyle,
                FilePath = filePath,
                GuildId = Context.Guild.Id
            });

            await tcsMessage.Task.Result.DeleteAsync();
            await FollowupAsync("Sound created.", ephemeral: true);
        }

        [SlashCommand("list", "Lists Soundboard sounds")]
        public async Task ListSounds()
        {
            try
            {
                var sounds = await _soundRepository.RetrieveByGuildIdAsync(Context.Guild.Id);

                var buttonChunks = sounds
                    .Select(sound => new SoundboardButton
                    {
                        SoundboardSoundId = sound.Id,
                        Label = sound.Label,
                        Emoji = sound.Emoji,
                        Style = (ButtonStyle)sound.ButtonStyle,
                        FilePath = sound.FilePath
                    }.ToDiscordButton())
                    .ToArray()
                    .Chunk(25);

                bool first = true;

                foreach (var chunk in buttonChunks)
                {
                    var builder = new ComponentBuilder();

                    foreach (var button in chunk)
                        builder.WithButton(button);

                    if (first)
                    {
                        await RespondAsync(components: builder.Build());
                        first = false;
                    }
                    else
                    {
                        await FollowupAsync(components: builder.Build());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}