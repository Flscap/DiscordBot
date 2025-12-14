using System.Diagnostics;

namespace DiscordBot.Audio.Mixer;

public static class PcmDecoder
{
    private const int FrameSize = 960 * 2 * 2; // 20ms @ 48kHz stereo

    public static IEnumerable<byte[]> Decode(string filePath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-loglevel quiet -nostdin -i \"{filePath}\" -f s16le -ac 2 -ar 48000 -",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        var stdout = process!.StandardOutput.BaseStream;

        byte[] buffer = new byte[FrameSize];

        while (true)
        {
            int read = stdout.Read(buffer, 0, FrameSize);
            if (read <= 0) break;

            if (read < FrameSize)
            {
                Array.Clear(buffer, read, FrameSize - read);
            }

            yield return buffer.ToArray();
        }
    }
}
