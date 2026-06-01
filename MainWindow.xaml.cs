using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace GLab6
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Year { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class Playlist
    {
        public string Name { get; set; }
        public ImageSource Cover { get; set; }
    }

    public partial class MainWindow : Window
    {
        public ObservableCollection<Song> Songs { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Songs = new ObservableCollection<Song>
            {
                new Song { Id = 1, Title = "Bohemian Rhapsody", Artist = "Queen", Album = "A Night at the Opera", Year = 1975, Duration = TimeSpan.FromSeconds(354) },
                new Song { Id = 2, Title = "siemanko", Artist = "some artist", Album = "cos tam", Year = 1971, Duration = TimeSpan.FromSeconds(482) },
            };

            Playlists = new ObservableCollection<Playlist>
            {
                new Playlist { Name = "Rock Classics", Cover = null },
                new Playlist { Name = "Workout", Cover = null }
            };

            DataContext = this;
        }

        private void CreateLibrary(object sender, RoutedEventArgs e) { }
        private void OpenLibrary(object sender, RoutedEventArgs e) { }
        private void SaveLibrary(object sender, RoutedEventArgs e) { }
        private void SongsDelete(object sender, RoutedEventArgs e) { }
        private void SongsEdit(object sender, RoutedEventArgs e) { }
        private void SongsImport(object sender, RoutedEventArgs e) { }

        private void NewPlaylist(object sender, RoutedEventArgs e)
        {
            PlaylistCreation dialog = new PlaylistCreation
            {
                Owner = this 
            };

            if (dialog.ShowDialog() == true)
            {
                Playlists.Add(new Playlist
                {
                    Name = dialog.PlaylistName,
                    Cover = dialog.CoverImage
                });
            }
        }
    }
}