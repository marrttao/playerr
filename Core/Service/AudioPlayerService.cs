using NAudio.Wave;
using playerr.domain.entities;
using playerr.Core.Providers;
using System;
using System.Threading.Tasks;

namespace playerr.Core.Service
{
    public class AudioPlayerService
    {
        private IWavePlayer _outputDevice;
        private AudioFileReader _audioFile;
        private EqualizerSampleProvider _eqProvider;
        private readonly AudioEffectService _effectService;

        public bool IsPlaying => _outputDevice?.PlaybackState == PlaybackState.Playing;

        public AudioPlayerService(AudioEffectService effectService)
        {
            _effectService = effectService;
        }

        // Асинхронное воспроизведение с эффектами
        public async Task PlayAsync(Track track)
        {
            Stop();

            try
            {
                if (!System.IO.File.Exists(track.Source))
                    throw new System.IO.FileNotFoundException("Трек не найден", track.Source);

                _audioFile = new AudioFileReader(track.Source);

                // Применяем эффекты
                var provider = _effectService.ApplyEffects(_audioFile);

                // Если это наш эквалайзер, сохраняем ссылку для управления
                if (provider is EqualizerSampleProvider eq)
                    _eqProvider = eq;
                else
                    _eqProvider = null;

                _outputDevice = new WaveOutEvent();
                _outputDevice.Init(provider);

                _outputDevice.PlaybackStopped += (s, e) =>
                {
                    _audioFile?.Dispose();
                    _outputDevice?.Dispose();
                    _audioFile = null;
                    _outputDevice = null;
                    _eqProvider = null;
                };

                _outputDevice.Play();
                Console.WriteLine($"Воспроизведение: {track.Name}");

                while (_outputDevice?.PlaybackState == PlaybackState.Playing)
                    await Task.Delay(200);
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
            _outputDevice?.Stop();
            _audioFile?.Dispose();
            _outputDevice?.Dispose();
            _audioFile = null;
            _outputDevice = null;
            _eqProvider = null;
        }

        // Регулировка громкости
        public void SetVolume(float volume)
        {
            if (_audioFile != null)
                _audioFile.Volume = Math.Clamp(volume, 0f, 1f);
        }

        // Управление эквалайзером на лету
        public void SetEqualizerBand(int band, float gain)
        {
            if (_eqProvider != null)
                _eqProvider.SetBandGain(band, gain);
        }
    }
}
