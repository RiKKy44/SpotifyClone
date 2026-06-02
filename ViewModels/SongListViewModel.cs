using GLab6.Commands;
using GLab6.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace GLab6.ViewModels
{
    public class SongListViewModel : BaseViewModel
    {
        private ObservableCollection<Song> _songs;

        private ObservableCollection<Playlist> _playlists;
        public ICollectionView FilteredSongs { get; }

        private string _searchText = string.Empty;

        private Song? _selectedSong;
        public ICommand ClearSearchCommand { get; }

        public ICommand AddToPlaylistCommand { get; }
        public string SearchText
        {
            get => _searchText;
            set
            {
                if(SetProperty(ref _searchText, value))
                {
                    FilteredSongs.Refresh();
                }
            }
        }

        public ObservableCollection<Playlist> Playlists
        {
            get => _playlists;
            set => SetProperty(ref _playlists, value);
        }
        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set => SetProperty(ref _songs, value);
        }
        public Song? SelectedSong
        {
            get => _selectedSong;
            set => SetProperty(ref _selectedSong, value);
        }

        public SongListViewModel(ObservableCollection<Song> songs, ObservableCollection<Playlist> playlists, ICommand addToPlaylistCommand)
        {
            _songs = songs;
            Playlists = playlists;

            AddToPlaylistCommand = addToPlaylistCommand;

            FilteredSongs = CollectionViewSource.GetDefaultView(_songs);
            FilteredSongs.Filter = FilterSongs;


            ClearSearchCommand = new RelayCommand(() => SearchText = string.Empty);
        }

        private bool FilterSongs(object item)
        {
            if(item is Song song)
            {
                if(string.IsNullOrEmpty(SearchText))
                    return true;
                string lowerQuery = SearchText.ToLower();

                return (song.Title?.ToLower().Contains(lowerQuery) == true) ||
                       (song.Artist?.ToLower().Contains(lowerQuery) == true) ||
                       (song.Album?.ToLower().Contains(lowerQuery) == true) ||
                       (song.Genre?.ToLower().Contains(lowerQuery) == true);
            }
            return false;
        }

    }
}