using Discord.Audio;
using DiscordBot.Audio.Mixer;

namespace DiscordBot.Audio.Discord;

public sealed class SoundboardAudioSource : IDisposable
{
    private readonly IAudioClient _audioClient;
    private readonly AudioOutStream _out;
    private readonly PcmMixer _mixer = new();

    private readonly CancellationTokenSource _cts = new();
    private readonly Task _pumpTask;

    private static readonly TimeSpan FrameDelay = TimeSpan.FromMilliseconds(20);

    public SoundboardAudioSource(IAudioClient audioClient)
    {
        _audioClient = audioClient;
        _out = audioClient.CreatePCMStream(AudioApplication.Mixed);

        _pumpTask = Task.Run(AudioPumpAsync);
    }

    public void Enqueue(string filePath)
    {
        _mixer.AddSound(filePath);
    }

    private async Task AudioPumpAsync()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var frame = _mixer.ReadMixedFrame();
                await _out.WriteAsync(frame, 0, frame.Length, _cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // normal shutdown
        }
        finally
        {
            await _out.FlushAsync();
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _pumpTask.Wait();
        _out.Dispose();
        _audioClient.Dispose();
    }
}
