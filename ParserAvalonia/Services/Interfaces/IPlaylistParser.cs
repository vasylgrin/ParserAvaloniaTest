using HtmlAgilityPack;
using ParserAvalonia.Models;

namespace ParserAvalonia.Services.Interfaces
{
    public interface IPlaylistParser
    {
        public Playlist GetPlaylistInfo(HtmlDocument document);
        public Song CreateSong(HtmlNodeCollection nodeCollection);
    }
}
