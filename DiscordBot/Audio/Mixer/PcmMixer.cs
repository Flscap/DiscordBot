using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace DiscordBot.Audio.Mixer;

public class PcmMixer
{
    private const int FrameSize = 960 * 2 * 2;
    private readonly int[] _mixBuffer = new int[FrameSize / 2];
    private readonly byte[] _outputBuffer = new byte[FrameSize];
    private readonly List<IEnumerator<byte[]>> _active = new();
    private readonly ConcurrentQueue<IEnumerator<byte[]>> _pending = new();
    private readonly List<IEnumerator<byte[]>> _finished = new();

    public void AddSound(string filePath)
    {
        _pending.Enqueue(PcmDecoder.Decode(filePath).GetEnumerator());
    }

    public byte[] ReadMixedFrame()
    {
        while (_pending.TryDequeue(out var gen))
            _active.Add(gen);

        if (_active.Count == 0)
        {
            Array.Clear(_outputBuffer);
            return _outputBuffer;
        }

        Array.Clear(_mixBuffer);

        _finished.Clear();

        foreach (var gen in _active)
        {
            if (!gen.MoveNext())
            {
                _finished.Add(gen);
                continue;
            }

            var shorts = MemoryMarshal.Cast<byte, short>(gen.Current);

            for (int i = 0; i < shorts.Length; i++)
                _mixBuffer[i] += shorts[i];
        }

        foreach (var f in _finished)
            _active.Remove(f);

        var outShorts = MemoryMarshal.Cast<byte, short>(_outputBuffer);

        for (int i = 0; i < outShorts.Length; i++)
            outShorts[i] = (short)Math.Clamp(_mixBuffer[i], short.MinValue, short.MaxValue);

        return _outputBuffer;
    }
}
