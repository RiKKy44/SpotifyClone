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

        private readonly Action _saveAction;
        public string Name => _playlist.Name;
        public byte[]? CoverData => _playlist.CoverData;

        public int SongCount => PlaylistSongs.Count;

        private Song? _selectedSong;

        public Song? SelectedSong
        {
            get => _selectedSong;
            set => SetProperty(ref _selectedSong, value);
        }
        public ObservableCollection<Song> PlaylistSongs { get; }

        public ICommand PlayCommand { get; }
        public ICommand RemoveSongCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }

        public PlaylistViewModel(Playlist playlist, ObservableCollection<Song> allSongs, Action saveAction, PlayerViewModel player)
        {
            _playlist = playlist;
            _saveAction = saveAction;

            var filtered = allSongs.Where(s => _playlist.SongIds.Contains(s.Id));

            PlaylistSongs = new ObservableCollection<Song>(filtered);

            PlayCommand = new RelayCommand(PlayPlaylist, () => PlaylistSongs.Count > 0);

            RemoveSongCommand = new RelayCommand(RemoveSong, () => SelectedSong != null);

            MoveUpCommand = new RelayCommand(MoveUp, () => SelectedSong != null);

            MoveDownCommand = new RelayCommand(MoveDown, () => SelectedSong != null);

            PlayCommand = new RelayCommand(() =>
            {
                var queue = new List<Song>(PlaylistSongs);
                player.PlayQueue(queue, 0);
            }, () => PlaylistSongs.Count > 0);
        }



        private void PlayPlaylist()
        {
        }   

        private void RemoveSong()
        {
            if(SelectedSong == null)
            {
                return;
            }
            _playlist.SongIds.Remove(SelectedSong.Id);
            PlaylistSongs.Remove(SelectedSong);
            OnPropertyChanged(nameof(SongCount));
            _saveAction();
        }

        private void MoveUp()
        {
            if (SelectedSong == null)
            {
                return;    
            }

            int index = PlaylistSongs.IndexOf(SelectedSong);

            if (index > 0)
            {
                int songId = _playlist.SongIds[index];

                _playlist.SongIds.RemoveAt(index);

                _playlist.SongIds.Insert(index - 1, songId);

                PlaylistSongs.Move(index, index - 1);

                _saveAction();
            }
        }

        private void MoveDown()
        {
            if (SelectedSong == null)
            {
                return;
            }
            int index = PlaylistSongs.IndexOf(SelectedSong);
            if (index < PlaylistSongs.Count - 1)
            {
                int songId = _playlist.SongIds[index];
                _playlist.SongIds.RemoveAt(index);
                _playlist.SongIds.Insert(index + 1, songId);
                PlaylistSongs.Move(index, index + 1);
                _saveAction();
            }
        }

    }
}
