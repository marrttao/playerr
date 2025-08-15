
using System;
using System.IO;
using System.Collections.Generic;
using playerr.domain.entities;

namespace playerr.Core.Service;

public interface IMusicService
{
    List<Track> GetTracks(string folderPath);
}

public class OfflineMusicService : IMusicService
{
    public List<Track> GetTracks(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException("Track folder not found.");

        var files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
        var tracks = new List<Track>();

        foreach (var file in files)
        {
            if (file.EndsWith(".mp3") || file.EndsWith(".wav"))
            {
                tracks.Add(new Track
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Source = file
                });
            }
        }

        return tracks;
    }
}