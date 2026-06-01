using System;

namespace GLab6.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Year { get; set; }
        public TimeSpan Duration { get; set; }

        //binary data for audio and cover
        public byte[]? AudioData { get; set; }

        public byte[]? CoverData { get; set; }
    }
}