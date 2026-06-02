using System.Windows;
using GLab6.ViewModels;

namespace GLab6
{
    public partial class PlaylistCreation : Window
    {
        public PlaylistCreation(PlaylistEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.CloseRequested += (result) =>
            {
                if (result.HasValue)
                {
                    this.DialogResult = result;
                }
                else
                {
                    this.Close();
                }
            };
        }
    }
}