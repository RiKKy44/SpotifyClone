using System.Windows;
using GLab6.ViewModels;

namespace GLab6
{
    public partial class SongEditWindow : Window
    {
        public SongEditWindow(SongEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.CloseRequested += (result) =>
            {
                if (result.HasValue) this.DialogResult = result;
                else this.Close();
            };
        }
    }
}