using HtmlAgilityPack;
using ParserAvalonia.Models;
using ParserAvalonia.Services.Interfaces;

namespace ParserAvalonia.Services
{
    public class AlbumParserService : IPlaylistParser
    {
        public Playlist GetPlaylistInfo(HtmlDocument document)
        {
            var header = document.DocumentNode.SelectSingleNode("//music-detail-header[@class='hydrated']");
            var avatar = header.GetAttributeValue("image-src", "Not Found");
            var name = header.GetAttributeValue("headline", "Not Found");
            var desc = header.GetAttributeValue("primary-text", "Not Found");

            return new Playlist
            {
                Name = name,
                Description = desc,
                Avatar = avatar,
            };
        }

        public Song CreateSong(HtmlNodeCollection nodeCollection)
        {
            return new Song
            {
                Name = nodeCollection[0].InnerText,
                Duration = nodeCollection[1].InnerText,
                Album = "-",
                Artist = "-",
            };
        }
    }
}
