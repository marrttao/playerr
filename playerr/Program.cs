using System;
using playerr.Core.Service;

class Program
{
    static void Main()
    {
        string musicFolder = @"D:\playerr\offlineMusic";
        var musicService = new OfflineMusicService();
        var getTracksUseCase = new GetTracksUseCase(musicService);
        var player = new AudioPlayerService();

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
            player.Play(selectedTrack);

            Console.WriteLine("Press P to pause/resume, S to stop, Q to return to list, T to exit.");

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
                        backToList = true;
                        break;
                    case ConsoleKey.T:
                        player.Stop();
                        return;
                }
            }
        }
    }
}