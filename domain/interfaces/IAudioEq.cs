namespace ApplicationLayer;

public interface IAudioEq
{
    ISampleProvider Apply(ISampleProvider input);
}
