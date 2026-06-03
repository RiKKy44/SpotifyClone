using System.Collections.ObjectModel;
using System.Windows.Input;
using GLab6.Commands;
using GLab6.Models;
using GLab6.Services;
using System.IO;
using Microsoft.Win32;
namespace GLab6.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentPage;
        private bool _isLibraryLoaded;
        private ObservableCollection<Song> _songs = new ObservableCollection<Song>();
        private ObservableCollection<Playlist> _playlists = new ObservableCollection<Playlist>();
        private Playlist? _selectedPlaylist;

        public PlayerViewModel Player { get; }
        public ICommand PlaySongCommand { get; }

        public BaseViewModel CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }


        public bool IsLibraryLoaded
        {
            get => _isLibraryLoaded;
            set
            {
                if(SetProperty(ref _isLibraryLoaded, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public Playlist? SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                SetProperty(ref _selectedPlaylist, value);
            }
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
        public ICommand ImportSongsCommand { get; }
        public ICommand NewLibraryCommand { get; }
        public ICommand OpenLibraryCommand { get; }
        public ICommand SaveLibraryCommand { get; }
        public ICommand NewPlaylistCommand { get; }

        public ICommand RenamePlaylistCommand { get; }
        public ICommand DeletePlaylistCommand { get; }
        public ICommand AddSongToPlaylistCommand { get; }

        public ICommand ShowPlaylistCommand { get; }
        public ICommand ShowAllSongsCommand { get; }
        public ICommand EditSongCommand { get; }
        public ICommand DeleteSongCommand { get; }
        public MainViewModel()
        {
            CurrentPage = new WelcomeViewModel(OnLibraryOpened, OnLibraryCreated);
            IsLibraryLoaded = false;

            NewLibraryCommand = new RelayCommand(CreateNewLibrary);
            OpenLibraryCommand = new RelayCommand(OpenExistingLibrary);
            SaveLibraryCommand = new RelayCommand(SaveLibrary, () => IsLibraryLoaded);
            NewPlaylistCommand = new RelayCommand(NewPlaylist, () => IsLibraryLoaded);

            ImportSongsCommand = new RelayCommand(ImportSongs, () => IsLibraryLoaded);

            RenamePlaylistCommand = new RelayCommand(param => RenamePlaylist(param as Playlist), _ => IsLibraryLoaded);
            DeletePlaylistCommand = new RelayCommand(param => DeletePlaylist(param as Playlist), _ => IsLibraryLoaded);
            AddSongToPlaylistCommand = new RelayCommand(param => AddSongToPlaylist(param as Playlist), _ => IsLibraryLoaded);
            
            EditSongCommand = new RelayCommand(param => EditSong(param as Song), _ => IsLibraryLoaded);
            
            DeleteSongCommand = new RelayCommand(param => DeleteSong(param as Song), _ => IsLibraryLoaded);
            ShowPlaylistCommand = new RelayCommand(param =>
            {
                if (param is Playlist p)
                {
                    SelectedPlaylist = p; 
                    CurrentPage = new PlaylistViewModel(p, Songs, SaveLibrary, Player); 
                }
            }, _ => IsLibraryLoaded);

            ShowAllSongsCommand = new RelayCommand(ShowAllSongs, () => IsLibraryLoaded);

            Player = new PlayerViewModel(new AudioPlayerService());
            PlaySongCommand = new RelayCommand(param => PlaySong(param as Song), _ => IsLibraryLoaded);
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

            CurrentPage = new SongListViewModel(Songs, Playlists, AddSongToPlaylistCommand);
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
            var vm = new PlaylistEditViewModel();
            PlaylistCreation win = new PlaylistCreation(vm) { Owner = System.Windows.Application.Current.MainWindow };
            if(win.ShowDialog() == true)
            {
                Playlists.Add(new Playlist { Name = vm.PlaylistName, CoverData = vm.CoverData });
                SaveLibrary();
            }
        }

        private void RenamePlaylist(Playlist? playlist)
        {
            if (playlist == null) return;

            var vm = new PlaylistEditViewModel(playlist.Name, playlist.CoverData);

            PlaylistCreation win = new PlaylistCreation(vm);

            if (win.ShowDialog() == true)
            {
                int index = Playlists.IndexOf(playlist);

                if (index >= 0)
                {
                    bool isCurrentlySelected = (CurrentPage is PlaylistViewModel pvm && pvm.Name == playlist.Name);

                    Playlists.RemoveAt(index);
                    playlist.Name = vm.PlaylistName;
                    playlist.CoverData = vm.CoverData;
                    Playlists.Insert(index, playlist);
                    if (isCurrentlySelected)
                    {
                        SelectedPlaylist = playlist;
                        CurrentPage = new PlaylistViewModel(playlist, Songs, SaveLibrary, Player);
                    }
                }
                SaveLibrary();
            }
        }


        private void DeletePlaylist(Playlist? playlist)
        {
            if (playlist == null) return;
            Playlists.Remove(playlist);
            CurrentPage = new SongListViewModel(Songs, Playlists, AddSongToPlaylistCommand);
            SaveLibrary();
        }


        private void AddSongToPlaylist(Playlist? playlist)
        {
            if (playlist == null)
            {
                return;
            }

            if(CurrentPage is SongListViewModel songListVm && songListVm.SelectedSong != null)
            {
                int index = Songs.IndexOf(songListVm.SelectedSong);

                if(index >= 0 && !playlist.SongIds.Contains(songListVm.SelectedSong.Id))
                {
                    playlist.SongIds.Add(songListVm.SelectedSong.Id);
                    SaveLibrary();  
                }
            }
        }

        private void ShowAllSongs()
        {
            SelectedPlaylist = null;

            CurrentPage = new SongListViewModel(Songs, Playlists, AddSongToPlaylistCommand);
        }
        private void EditSong(Song? song)
        {
            if (song == null) return;

            var vm = new SongEditViewModel(song.Title, song.Artist, song.Album, song.Genre, song.Year, song.CoverData);
            var win = new SongEditWindow(vm);

            if (win.ShowDialog() == true)
            {
                int index = Songs.IndexOf(song);
                if (index >= 0)
                {
                    Songs.RemoveAt(index);

                    song.Title = vm.Title;
                    song.Artist = vm.Artist;
                    song.Album = vm.Album;
                    song.Genre = vm.Genre;
                    song.Year = vm.Year;
                    song.CoverData = vm.CoverData;

                    Songs.Insert(index, song);

                    if (SelectedPlaylist != null) CurrentPage = new PlaylistViewModel(SelectedPlaylist, Songs, SaveLibrary, Player);
                    else CurrentPage = new SongListViewModel(Songs, Playlists, AddSongToPlaylistCommand);
                }
                SaveLibrary();
            }
        }
        private void DeleteSong(Song? song)
        {
            if (song == null) return;

            foreach (var playlist in Playlists)
            {
                if (playlist.SongIds.Contains(song.Id))
                {
                    playlist.SongIds.Remove(song.Id);
                }
            }

            Songs.Remove(song);

            if (SelectedPlaylist != null) CurrentPage = new PlaylistViewModel(SelectedPlaylist, Songs, SaveLibrary,Player);
            else CurrentPage = new SongListViewModel(Songs, Playlists, AddSongToPlaylistCommand);

            SaveLibrary();
        }
        private void ImportSongs()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.wav;*.flac;*.aac;*.ogg|All Files|*.*",
                Title = "Choose songs to import"
            };

            if(ofd.ShowDialog() == true)
            {
                foreach(var file in ofd.FileNames)
                {
                    try
                    {
                        var tfile = TagLib.File.Create(file);

                        var song = new Song
                        {
                            Id = Songs.Count > 0 ? Songs.Max(s => s.Id) + 1 : 1,
                            Title = string.IsNullOrWhiteSpace(tfile.Tag.Title) ? Path.GetFileNameWithoutExtension(file) : tfile.Tag.Title,
                            Artist = tfile.Tag.FirstPerformer ?? "Unknown Artist",
                            Album = tfile.Tag.Album ?? "Unknown Album",
                            Genre = tfile.Tag.FirstGenre ?? "Unknown Genre",
                            Year = (int)tfile.Tag.Year,
                            Duration = tfile.Properties.Duration,
                            AudioData = File.ReadAllBytes(file)
                        };

                        if(tfile.Tag.Pictures.Length > 0)
                        {
                            song.CoverData = tfile.Tag.Pictures[0].Data.Data;
                        }
                        Songs.Add(song);
                    }
                    catch
                    {

                    }

                }
                SaveLibrary();
            }
        }
        private void CreateNewLibrary()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Musica Library (*.musica)|*.musica|JSON Files (*.json)|*.json",
                Title = "Create New Music Library"
            };

            if (sfd.ShowDialog() == true)
            {
                OnLibraryCreated(sfd.FileName);
            }
        }

        private void OpenExistingLibrary()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Musica Library (*.musica)|*.musica|JSON Files (*.json)|*.json",
                Title = "Open Music Library"
            };

            if (ofd.ShowDialog() == true)
            {
                OnLibraryOpened(ofd.FileName);
            }
        }
        private void PlaySong(Song? song)
        {
            if (song == null) return;

            List<Song> queue;

            if (CurrentPage is PlaylistViewModel pvm)
            {
                queue = new List<Song>(pvm.PlaylistSongs);
            }
            else queue = new List<Song>(Songs);

            int index = queue.IndexOf(song);
            Player.PlayQueue(queue, index);
        }
    }
}