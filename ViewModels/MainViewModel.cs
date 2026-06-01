using System.Collections.ObjectModel;
using System.Windows.Input;
using GLab6.Commands;
using GLab6.Models;
using GLab6.Services;

namespace GLab6.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentPage;
        private bool _isLibraryLoaded;
        private ObservableCollection<Song> _songs = new ObservableCollection<Song>();
        private ObservableCollection<Playlist> _playlists = new ObservableCollection<Playlist>();

        public BaseViewModel CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public bool IsLibraryLoaded
        {
            get => _isLibraryLoaded;
            set => SetProperty(ref _isLibraryLoaded, value);
        }

        public ObservableCollection<Song> Songs
        {
            get => _songs;
            set => SetProperty(ref _songs, value);
        }

        public ObservableCollection<Playlist> Playlists
        {
            get => _playlists;
            set => SetProperty(ref _playlists, value);
        }

        public ICommand NewLibraryCommand { get; }
        public ICommand OpenLibraryCommand { get; }
        public ICommand SaveLibraryCommand { get; }
        public ICommand NewPlaylistCommand { get; }

        public MainViewModel()
        {
            CurrentPage = new WelcomeViewModel(OnLibraryOpened, OnLibraryCreated);
            IsLibraryLoaded = false;

            NewLibraryCommand = new RelayCommand(() => CurrentPage = new WelcomeViewModel(OnLibraryOpened, OnLibraryCreated));
            OpenLibraryCommand = new RelayCommand(() => CurrentPage = new WelcomeViewModel(OnLibraryOpened, OnLibraryCreated));
            SaveLibraryCommand = new RelayCommand(SaveLibrary, () => IsLibraryLoaded);
            NewPlaylistCommand = new RelayCommand(NewPlaylist, () => IsLibraryLoaded);
        }

        private void OnLibraryCreated(string filePath)
        {
            LibraryService.CreateNew(filePath);
            LoadLibraryData(new MusicLibrary());
        }

        private void OnLibraryOpened(string filePath)
        {
            var library = LibraryService.Load(filePath);
            LoadLibraryData(library);
        }

        private void LoadLibraryData(MusicLibrary library)
        {
            Songs = new ObservableCollection<Song>(library.Songs);
            Playlists = new ObservableCollection<Playlist>(library.Playlists);
            IsLibraryLoaded = true;

            CurrentPage = new SongListViewModel(Songs);
        }

        public void SaveLibrary()
        {
            if (!IsLibraryLoaded) return;
            var library = new MusicLibrary
            {
                Songs = new List<Song>(Songs),
                Playlists = new List<Playlist>(Playlists)
            };
            LibraryService.Save(library);
        }

        private void NewPlaylist()
        {
            PlaylistCreation win2 = new PlaylistCreation { Owner = System.Windows.Application.Current.MainWindow };
            if (win2.ShowDialog() == true)
            {
                Playlists.Add(new Playlist
                {
                    Name = win2.PlaylistName,
                    CoverData = win2.CoverData 
                });
                SaveLibrary();
            }
        }
    }
}