using HtmlAgilityPack;
using ParserAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ParserAvalonia.Services.Interfaces;

namespace ParserAvalonia.Services
{
    internal class SongParserService
    {
        private static IPlaylistParser _playlistParser;
        private static readonly FetchDataService _fetchDataService = new();
        private static string _parseMode = "";

        public static Playlist GetPlaylist(string url)
        {
            var isOk = SetParserMode(url);
            if (!isOk) return null;
            
            var pageSource = _fetchDataService.FetchData(url);
            return Parse(pageSource, _parseMode);
        }

        private static bool SetParserMode(string url)
        {
            if (url.Contains("playlist"))
            {
                _parseMode = "image";
                _playlistParser = new PlaylistParserService();
                return true;
            }
            else if (url.Contains("album"))
            {
                _parseMode = "text";
                _playlistParser = new AlbumParserService();
                return true;
            }

            return false;   
        }

        private static Playlist Parse(string pageSource, string parseMode)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            var songNodes = htmlDoc.DocumentNode.SelectNodes($"//music-{parseMode}-row[@class='hydrated']");
            if (songNodes is null) return null;

            var songsCollection = CreateSongsCollection(songNodes, _playlistParser.CreateSong).ToList();
            var uniqSongs = new HashSet<Song>(songsCollection).ToList();

            var playlist = _playlistParser.GetPlaylistInfo(htmlDoc);
            playlist.Songs = uniqSongs;
            return playlist;
        }

        private static IEnumerable<Song> CreateSongsCollection(HtmlNodeCollection songNodes, Func<HtmlNodeCollection, Song> func)
        {
            if (songNodes is null) yield break;
            foreach (var songNode in songNodes)
            {
                var musicLinks = songNode.SelectNodes(".//music-link[@title]");
                if (musicLinks is null) yield break;

                var song = func.Invoke(musicLinks);
                song.Index = songNode.SelectSingleNode(".//span[@class='index']").InnerText;

                yield return song;
            }
        }
    }
}
