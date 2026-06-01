using System.Collections.Generic;

namespace GLab6.Models
{
    public class Playlist
    {
        public string Name { get; set; } = string.Empty;

        public byte[]? CoverData { get; set; }

        public List<int> SongIds { get; set; } = new List<int>();
    }
}