using System;
using System.Threading.Tasks;
using playerr.Core.Service;
using playerr.domain.entities;

class Program
{
    static async Task Main()
    {
        string musicFolder = @"D:\playerr\offlineMusic";

        var musicService = new OfflineMusicService();
        var getTracksUseCase = new GetTracksUseCase(musicService);

        var effectService = new AudioEffectService();
        var eq = new EqualizerEffect(44100); // создаем эквалайзер для 44.1 кГц
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
            if (input?.Trim().ToUpper() == "T")
                return;

            if (!int.TryParse(input, out int choice) || choice < 1 || choice > tracks.Count)
            {
                Console.WriteLine("Invalid selection!");
                continue;
            }

            var selectedTrack = tracks[choice - 1];

            // Запускаем трек асинхронно
            var playTask = player.PlayAsync(selectedTrack);

            Console.WriteLine("Controls: P - pause/resume, S - stop, Q - back to list, T - exit");
            bool backToList = false;
            while (!backToList)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.P:
                        if (player.IsPlaying)
                            player.Pause();
                        else
                            player.Resume();
                        break;
                    case ConsoleKey.S:
                        player.Stop();
                        break;
                    case ConsoleKey.Q:
                        player.Stop();
                        backToList = true;
                        break;
                    case ConsoleKey.T:
                        player.Stop();
                        return;
                    case ConsoleKey.D1: eq.SetBandGain(0, 6f); break; // пример управления басом
                    case ConsoleKey.D2: eq.SetBandGain(1, -3f); break; // средние
                    case ConsoleKey.D3: eq.SetBandGain(2, 2f); break;  // высокие
                }
            }

            // Ждём окончания воспроизведения, если пользователь вернулся в список
            await playTask;
        }
    }
}
