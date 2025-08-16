using NAudio.Wave;
namespace domain;

public interface IAudioEq
{
    ISampleProvider Apply(ISampleProvider source);
}
