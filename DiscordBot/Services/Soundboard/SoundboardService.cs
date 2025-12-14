using Discord.WebSocket;
using DiscordBot.Audio.Discord;
using System.Collections.Concurrent;

namespace DiscordBot.Services.Soundboard;

public class SoundboardService
{
    private readonly ConcurrentDictionary<ulong, Task<SoundboardAudioSource>> _sources = new();

    public Task<SoundboardAudioSource> GetOrCreateAsync(SocketGuild guild, SocketVoiceChannel channel)
    {
        return _sources.GetOrAdd(
            guild.Id,
            _ => CreateSourceAsync(channel));
    }

    private async Task<SoundboardAudioSource> CreateSourceAsync(SocketVoiceChannel channel)
    {
        var client = await channel.ConnectAsync();
        return new SoundboardAudioSource(client);
    }

    public void Stop(ulong guildId)
    {
        if (_sources.TryRemove(guildId, out var source))
            source.Dispose();
    }
}
