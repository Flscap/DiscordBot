using System.Runtime.InteropServices;

namespace DiscordBot.Audio.Mixer;

public class PcmMixer
{
    private const int FrameSize = 960 * 2 * 2;
    private readonly List<IEnumerator<byte[]>> _active = new();

    public void AddSound(string filePath)
    {
        _active.Add(PcmDecoder.Decode(filePath).GetEnumerator());
    }

    public byte[] ReadMixedFrame()
    {
        if (_active.Count == 0)
            return new byte[FrameSize]; // silence

        int samplesCount = FrameSize / 2;
        var mix = new int[samplesCount];
        var finished = new List<IEnumerator<byte[]>>();

        foreach (var gen in _active)
        {
            if (!gen.MoveNext())
            {
                finished.Add(gen);
                continue;
            }

            var frame = gen.Current;
            var shorts = MemoryMarshal.Cast<byte, short>(frame);

            for (int i = 0; i < shorts.Length; i++)
                mix[i] += shorts[i];
        }

        foreach (var f in finished)
            _active.Remove(f);

        // clamp
        var output = new byte[FrameSize];
        var outShorts = MemoryMarshal.Cast<byte, short>(output);

        for (int i = 0; i < outShorts.Length; i++)
            outShorts[i] = (short)Math.Clamp(mix[i], short.MinValue, short.MaxValue);

        return output;
    }
}
