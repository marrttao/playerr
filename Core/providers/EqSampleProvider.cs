using NAudio.Wave;
using NAudio.Dsp;

namespace playerr.Core.Providers;

public class EqualizerEffect : IAudioEq
{
    private readonly BiQuadFilter[] filters;
    private readonly float[] freqs = { 100f, 1000f, 5000f };
    private readonly float[] qs = { 1f, 1f, 1f };
    private readonly int sampleRate;

    public EqualizerEffect(int sampleRate)
    {
        this.sampleRate = sampleRate;
        filters = new BiQuadFilter[3]; // low, mid, high
        for (int i = 0; i < filters.Length; i++)
        {
            filters[i] = BiQuadFilter.PeakingEQ(sampleRate, freqs[i], qs[i], 0f);
        }
    }

    public ISampleProvider Apply(ISampleProvider input)
    {
        return new EqualizerSampleProvider(input, filters, this);
    }

    public void SetBandGain(int bandIndex, float gain)
    {
        if (bandIndex < 0 || bandIndex >= filters.Length) return;

        filters[bandIndex] = BiQuadFilter.PeakingEQ(
            sampleRate,
            freqs[bandIndex],
            qs[bandIndex],
            gain
        );
    }
}

public class EqualizerSampleProvider : ISampleProvider
{
    private readonly ISampleProvider source;
    private readonly BiQuadFilter[] filters;
    private readonly EqualizerEffect eqEffect;

    public EqualizerSampleProvider(ISampleProvider source, BiQuadFilter[] filters, EqualizerEffect eqEffect)
    {
        this.source = source;
        this.filters = filters;
        this.eqEffect = eqEffect;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = source.Read(buffer, offset, count);

        for (int n = 0; n < samplesRead; n++)
        {
            float sample = buffer[offset + n];
            foreach (var filter in filters)
                sample = filter.Transform(sample);
            buffer[offset + n] = sample;
        }

        return samplesRead;
    }

    public WaveFormat WaveFormat => source.WaveFormat;

    public void SetBandGain(int band, float gain)
    {
        if (eqEffect != null)
        {
            eqEffect.SetBandGain(band, gain);
        }
    }
}