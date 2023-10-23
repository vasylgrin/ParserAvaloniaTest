using System.Collections.Generic;

namespace ParserAvalonia.Models
{
    public sealed class Playlist
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Description { get; set; }
        public List<Song> Songs { get; set; } = new();
    }
}
