using System.Collections.ObjectModel;
using GLab6.Models;

namespace GLab6.ViewModels
{
    public class SongListViewModel : BaseViewModel
    {
        private ObservableCollection<Song> _songs;
        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set => SetProperty(ref _songs, value);
        }

        public SongListViewModel(ObservableCollection<Song> songs)
        {
            _songs = songs;
        }
    }
}