using System;
using System.Threading.Tasks;
using playerr.Core.Service;
using playerr.domain.entities;
using playerr.Core.Providers;

class Program
{
    static async Task Main()
    {
        string musicFolder = @"D:\playerr\offlineMusic";

        var musicService = new OfflineMusicService();
        var getTracksUseCase = new GetTracksUseCase(musicService);

        var effectService = new AudioEffectService();
        var eq = new EqualizerEffect(44100);
        effectService.AddEffect(eq);

        var player = new AudioPlayerService(effectService);

        while (true)
        {
            var tracks = getTracksUseCase.Execute(musicFolder);

            if (tracks.Count == 0)
            {
                Console.WriteLine("No tracks available for playback!");
                return;
            }

            Console.WriteLine("Track list:");
            for (int i = 0; i < tracks.Count; i++)
                Console.WriteLine($"{i + 1}. {tracks[i].Name}");

            Console.Write("Select track number (or T to exit): ");
            var input = Console.ReadLine();
            if (input?.Trim().ToUpper() == "T") return;

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > tracks.Count)
            {
                Console.WriteLine("Invalid selection!");
                continue;
            }

            var selectedTrack = tracks[choice - 1];
            var playTask = player.PlayAsync(selectedTrack);

            Console.WriteLine("Controls: P - pause/resume, S - stop, Q - back to list, T - exit");
            bool backToList = false;
            while (!backToList)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.P:
                        if (player.IsPlaying) player.Pause();
                        else player.Resume();
                        break;
                    case ConsoleKey.S:
                        player.Stop();
                        break;
                    case ConsoleKey.Q:
                        player.Stop();
                        backToList = true;
                        break;
                    case ConsoleKey.Y:
                        Console.WriteLine("Настройка эквалайзера");
                        for (int band = 0; band < 3; band++)
                        {
                            Console.Write($"Введите усиление для полосы {band + 1} (dB, -12..12): ");
                            var bandInput = Console.ReadLine();
                            if (float.TryParse(bandInput, out float gain))
                                eq.SetBandGain(band, Math.Clamp(gain, -12f, 12f));
                        }

                        break;
                        Console.WriteLine("Эквалайзер настроен. Запуск трека...");
                    case ConsoleKey.T:
                        player.Stop();
                        return;
                }
            }

            await playTask;
        }
    }
}