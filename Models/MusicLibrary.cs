using System.Collections.Generic;

namespace GLab6.Models
{
    public class MusicLibrary
    {
        public List<Song> Songs { get; set; } = new List<Song>();
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}