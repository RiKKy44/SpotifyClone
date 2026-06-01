using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GLab6
{
    public partial class PlaylistCreation : Window
    {        public string PlaylistName => PlaylistNameTextBox.Text;
        public ImageSource CoverImage => servicePhoto.Source;

        public PlaylistCreation()
        {
            InitializeComponent();
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter = "Image Files|*.jpg;*.jpeg;*.png"
            };

            if (op.ShowDialog() == true)
            {
                servicePhoto.Source = new BitmapImage(new Uri(op.FileName));
            }
        }

        private void PlaylistNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SaveBtn.IsEnabled = !string.IsNullOrWhiteSpace(PlaylistNameTextBox.Text);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}