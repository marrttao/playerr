using System.Collections.Generic;
using playerr.Core.Service;
using playerr.domain.entities;

public class GetTracksUseCase
{
    private readonly IMusicService _musicService;

    public GetTracksUseCase(IMusicService musicService)
    {
        _musicService = musicService;
    }

    /// <summary>
    /// Получение треков, оффлайн или онлайн.
    /// Для оффлайн можно передать путь к папке, для онлайн - источник из API.
    /// </summary>
    public List<Track> Execute(string folderPath = null)
    {
        var tracks = new List<Track>();

        if (!string.IsNullOrEmpty(folderPath))
        {
            // Получаем локальные треки
            tracks.AddRange(_musicService.GetTracks(folderPath));
        }

        // В будущем можно добавить вызов онлайн-сервисов
        // tracks.AddRange(_musicService.GetOnlineTracks());

        return tracks;
    }
}