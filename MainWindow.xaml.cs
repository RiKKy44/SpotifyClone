using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using GLab6.ViewModels;
using GLab6.Models;

namespace GLab6
{

    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

    }
}