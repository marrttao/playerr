namespace ApplicationLayer;

public class EqualizerEffect : IAudioEffect
{
    private readonly BiQuadFilter[] filters;

    public EqualizerEffect(int sampleRate)
    {
        filters = new BiQuadFilter[3]; // низкие, средние, высокие
        filters[0] = BiQuadFilter.PeakingEQ(sampleRate, 100f, 1f, 0f);
        filters[1] = BiQuadFilter.PeakingEQ(sampleRate, 1000f, 1f, 0f);
        filters[2] = BiQuadFilter.PeakingEQ(sampleRate, 5000f, 1f, 0f);
    }

    public ISampleProvider Apply(ISampleProvider input)
    {
        return new EqualizerSampleProvider(input, filters);
    }

    public void SetBandGain(int bandIndex, float gain)
    {
        if (bandIndex < 0 || bandIndex >= filters.Length) return;

        filters[bandIndex].SetPeakingEq(filters[bandIndex].Frequency,
            filters[bandIndex].Q,
            gain);
    }
}

// Внутренний класс для обработки сэмплов
public class EqualizerSampleProvider : ISampleProvider
{
    private readonly ISampleProvider source;
    private readonly BiQuadFilter[] filters;

    public EqualizerSampleProvider(ISampleProvider source, BiQuadFilter[] filters)
    {
        this.source = source;
        this.filters = filters;
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
}