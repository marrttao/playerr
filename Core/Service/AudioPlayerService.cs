using NAudio.Wave;
using playerr.domain.entities;
using System;
using System.Threading.Tasks;

namespace playerr.Core.Service
{
    public class AudioPlayerService
    {
        private IWavePlayer _outputDevice;
        private ISampleProvider _currentProvider;
        private AudioFileReader _audioFile;
        private readonly AudioEffectService _effectService;

        public bool IsPlaying => _outputDevice?.PlaybackState == PlaybackState.Playing;

        public AudioPlayerService(AudioEffectService effectService)
        {
            _effectService = effectService;
        }

        // Асинхронное воспроизведение с поддержкой эффектов
        public async Task PlayAsync(Track track)
        {
            Stop();

            try
            {
                if (!System.IO.File.Exists(track.Source))
                    throw new System.IO.FileNotFoundException("Трек не найден", track.Source);

                _audioFile = new AudioFileReader(track.Source);
                _currentProvider = _effectService.ApplyEffects(_audioFile);

                _outputDevice = new WaveOutEvent();
                _outputDevice.Init(_currentProvider);

                // Событие завершения трека
                _outputDevice.PlaybackStopped += (s, e) =>
                {
                    _audioFile?.Dispose();
                    _outputDevice?.Dispose();
                    _audioFile = null;
                    _outputDevice = null;
                    _currentProvider = null;
                };

                _outputDevice.Play();

                Console.WriteLine($"Воспроизведение: {track.Name}");

                // Асинхронно ждём завершения воспроизведения
                while (_outputDevice?.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(200); // не блокируем главный поток
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка воспроизведения: {ex.Message}");
            }
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
            if (_outputDevice != null)
            {
                _outputDevice.Stop();
                _outputDevice.Dispose();
                _outputDevice = null;
            }

            if (_audioFile != null)
            {
                _audioFile.Dispose();
                _audioFile = null;
            }

            _currentProvider = null;
        }

        // Регулировка громкости
        public void SetVolume(float volume)
        {
            if (_audioFile != null)
                _audioFile.Volume = Math.Clamp(volume, 0f, 1f);
        }
    }
}
