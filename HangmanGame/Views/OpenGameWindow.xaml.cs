using HangmanGame.Models;
using System.Windows;

namespace HangmanGame.Views
{
    public partial class OpenGameWindow : Window
    {
        public SavedGame SelectedGame { get; private set; }

        public OpenGameWindow()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModels.OpenGameViewModel;
            if (vm?.SelectedGame == null)
            {
                MessageBox.Show("Please select a game to open.", "No Game Selected",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelectedGame = vm.SelectedGame;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}