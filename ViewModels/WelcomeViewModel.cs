using System;
using System.Windows.Input;
using Microsoft.Win32;
using GLab6.Commands;

namespace GLab6.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly Action<string> _onLibraryOpened;
        private readonly Action<string> _onLibraryCreated;

        public ICommand CreateLibraryCommand { get; }
        public ICommand OpenLibraryCommand { get; }

        public WelcomeViewModel(Action<string> onLibraryOpened, Action<string> onLibraryCreated)
        {
            _onLibraryOpened = onLibraryOpened;
            _onLibraryCreated = onLibraryCreated;

            CreateLibraryCommand = new RelayCommand(CreateLibrary);
            OpenLibraryCommand = new RelayCommand(OpenLibrary);
        }

        private void CreateLibrary()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Musica Library (*.musica)|*.musica|JSON Files (*.json)|*.json",
                Title = "Create New Music Library"
            };
            if (sfd.ShowDialog() == true) _onLibraryCreated(sfd.FileName);
        }

        private void OpenLibrary()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Musica Library (*.musica)|*.musica|JSON Files (*.json)|*.json",
                Title = "Open Music Library"
            };
            if (ofd.ShowDialog() == true) _onLibraryOpened(ofd.FileName);
        }
    }
}