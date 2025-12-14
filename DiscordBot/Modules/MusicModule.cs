using Discord;
using Discord.Audio;
using Discord.Interactions;
using DiscordBot.Services.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DiscordBot.Modules;

[Discord.Interactions.Group("music", "Music related commands")]
public class MusicModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILoggingService _logger;

    public MusicModule(ILoggingService loggingService)
    {
        _logger = loggingService;
    }

    [SlashCommand("play", "Play music from youtube")]
    public async Task PlayAsync(
        [Discord.Interactions.Summary(description: "Youtube URL or search query")] string query)
    {
        await DeferAsync(); // Equivalent to interaction.response.defer()

        var userVoiceChannel = (Context.User as IGuildUser)?.VoiceChannel;
        if (userVoiceChannel == null)
        {
            await FollowupAsync("❌ You must be in a voice channel!");
            return;
        }

        var audioClient = Context.Guild.AudioClient ?? await userVoiceChannel.ConnectAsync();

        // Find YouTube stream URL
        var ytSearch = await GetAudioUrlAsync(query);
        if (ytSearch == null)
        {
            await FollowupAsync("❌ Youtube search failed");
            return;
        }

        YoutubeDLResult ytSearchResult = JsonConvert.DeserializeObject<YoutubeDLResult>(ytSearch);

        var ffmpeg = CreateStream(ytSearchResult.Url);
        var output = audioClient.CreatePCMStream(AudioApplication.Music);

        _ = Task.Run(async () =>
        {
            await ffmpeg.StandardOutput.BaseStream.CopyToAsync(output);
            await output.FlushAsync();
        });

        await FollowupAsync($"🎶 Playing: {ytSearchResult.Title}");
    }

    [SlashCommand("stop", "Stop playback")]
    public async Task StopAsync()
    {
        var audioClient = Context.Guild.AudioClient;
        if (audioClient == null)
        {
            await RespondAsync("❌ Nothing is playing.");
            return;
        }

        await audioClient.StopAsync();
        await RespondAsync("⏹️ Stopped playback.");
    }

    // FFmpeg process helper
    private Process? CreateStream(string url)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{url}\" -f s16le -ar 48000 -ac 2 pipe:1",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });
    }

    // Get audio URL from query (using yt-dlp)
    private async Task<string?> GetAudioUrlAsync(string query)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"./External/ytsearch.py \"{query}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi)!;
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        string sourceUrl = output.Trim();
        return sourceUrl;
    }

    private class YoutubeDLResult
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }
}