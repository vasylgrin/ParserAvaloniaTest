using System;

namespace ParserAvalonia.Models
{
    public sealed class Song
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Duration { get; set; }


        public override bool Equals(object obj)
        {
            return Equals(obj as Song);
        }

        public bool Equals(Song song)
        {
            return song != null 
                   && Index == song.Index 
                   && Name == song.Name 
                   && Artist == song.Artist 
                   && Album == song.Album 
                   && Duration == song.Duration;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, Name, Artist, Album, Duration);
        }
    }
}
