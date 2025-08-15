using NAudio.Wave;
using playerr.domain.entities;

namespace playerr.Core.Service;

public class AudioPlayerService
{
    private IWavePlayer _outputDevice;
    private AudioFileReader _audioFile;

    public bool IsPlaying => _outputDevice?.PlaybackState == PlaybackState.Playing;

    public void Play(Track track)
    {
        Stop();
        _audioFile = new AudioFileReader(track.Source);
        _outputDevice = new WaveOutEvent();
        _outputDevice.Init(_audioFile);
        _outputDevice.Play();
    }

    public void Pause()
    {
        if (_outputDevice?.PlaybackState == PlaybackState.Playing)
            _outputDevice.Pause();
    }

    public void Resume()
    {
        if (_outputDevice?.PlaybackState == PlaybackState.Paused)
            _outputDevice.Play();
    }

    public void Stop()
    {
        _outputDevice?.Stop();
        _outputDevice?.Dispose();
        _outputDevice = null;
        _audioFile?.Dispose();
        _audioFile = null;
    }
}