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

        public ICollectionView FilteredSongs { get; }

        private string _searchText = string.Empty;

        public ICommand ClearSearchCommand { get; }
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
        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set => SetProperty(ref _songs, value);
        }

        public SongListViewModel(ObservableCollection<Song> songs)
        {
            _songs = songs;

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