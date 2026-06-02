using System.Collections.ObjectModel;
using System.Windows.Input;
using GLab6.Commands;
using GLab6.Models;
using GLab6.Services;
using System.Linq;
using System.IO;
using Microsoft.Win32;
using GLab6.Views;

namespace GLab6.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel _currentPage;
        private bool _isLibraryLoaded;
        private ObservableCollection<Song> _songs = new ObservableCollection<Song>();
        private ObservableCollection<Playlist> _playlists = new ObservableCollection<Playlist>();
        private Playlist? _selectedPlaylist;
        
        public ICommand ImportSongsCommand { get; }

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
                if (SetProperty(ref _selectedPlaylist, value) && value != null)
                {
                    CurrentPage = new PlaylistViewModel(value, Songs);
                }
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

            ImportSongsCommand = new RelayCommand(ImportSongs, () => IsLibraryLoaded);
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
                    catch(Exception ex)
                    {

                    }

                }
                SaveLibrary();
            }
        }
    }
}