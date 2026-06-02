using GLab6.Commands;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;

namespace GLab6.ViewModels
{
    public class SongEditViewModel : BaseViewModel
    {
        private string _title = string.Empty;
        private string _artist = string.Empty;
        private string _album = string.Empty;
        private string _genre = string.Empty;
        private int _year;
        private byte[]? _coverData;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string Artist { get => _artist; set => SetProperty(ref _artist, value); }
        public string Album { get => _album; set => SetProperty(ref _album, value); }
        public string Genre { get => _genre; set => SetProperty(ref _genre, value); }
        public int Year { get => _year; set => SetProperty(ref _year, value); }

        public byte[]? CoverData { get => _coverData; set => SetProperty(ref _coverData, value); }

        public event Action<bool?>? CloseRequested;

        public ICommand BrowseCoverCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public SongEditViewModel(string title, string artist, string album, string genre, int year, byte[]? coverData)
        {
            Title = title ?? "Unknown";
            Artist = artist ?? "Unknown";
            Album = album ?? "Unknown";
            Genre = genre ?? "Unknown";
            Year = year;
            CoverData = coverData;

            BrowseCoverCommand = new RelayCommand(BrowseCover);
            SaveCommand = new RelayCommand(Save, () => !string.IsNullOrWhiteSpace(Title));
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save() => CloseRequested?.Invoke(true);
        private void Cancel() => CloseRequested?.Invoke(false);

        private void BrowseCover()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select Song Cover",
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };

            if (ofd.ShowDialog() == true)
            {
                CoverData = File.ReadAllBytes(ofd.FileName);
            }
        }
    }
}