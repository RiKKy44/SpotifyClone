using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GLab6.Commands;
using GLab6.Models;

namespace GLab6.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        private readonly Playlist _playlist;

        public string Name => _playlist.Name;
        public byte[]? CoverData => _playlist.CoverData;
        public int SongCount => PlaylistSongs.Count;
        public ObservableCollection<Song> PlaylistSongs { get; }

        public ICommand PlayCommand { get; }

        public PlaylistViewModel(Playlist playlist, ObservableCollection<Song> allSongs)
        {
            _playlist = playlist;

            var filtered = allSongs.Where(s => _playlist.SongIds.Contains(s.Id));

            PlaylistSongs = new ObservableCollection<Song>(filtered);

            PlayCommand = new RelayCommand(PlayPlaylist, () => PlaylistSongs.Count > 0);
        }

        private void PlayPlaylist()
        {

        }
    }
}