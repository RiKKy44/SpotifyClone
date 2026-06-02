using GLab6.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;

namespace GLab6.ViewModels
{
    public class PlaylistEditViewModel : BaseViewModel
    {
        private string _playlistName = string.Empty;

        private byte[]? _coverData;

        public event Action<bool?>? CloseRequested;
        public ICommand BrowseCoverCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public string PlaylistName
        {
            get => _playlistName;
            set
            {
                if(SetProperty(ref _playlistName, value))
                {
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public byte[]? CoverData
        {
            get => _coverData;
            set => SetProperty(ref _coverData, value);
        }

        public PlaylistEditViewModel(string initialName = "", byte[]? initialCover = null)
        {
            PlaylistName = initialName;
            CoverData = initialCover;

            BrowseCoverCommand = new RelayCommand(BrowseCover);
            SaveCommand = new RelayCommand(Save, () => !string.IsNullOrWhiteSpace(PlaylistName));
            CancelCommand = new RelayCommand(Cancel);
        }

        private void BrowseCover()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select Playlist Cover",
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };

            if (ofd.ShowDialog() == true)
            {
                CoverData = File.ReadAllBytes(ofd.FileName);    
            }
        }

        private void Save()
        {
            CloseRequested?.Invoke(true);
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(false);
        }
    }
}
