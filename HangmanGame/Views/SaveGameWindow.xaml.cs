using System.Windows;

namespace HangmanGame.Views
{
    public partial class SaveGameWindow : Window
    {
        public string GameName { get; private set; }

        public SaveGameWindow()
        {
            InitializeComponent();
            GameNameTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = GameNameTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a name for the game.", "Invalid Name",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GameName = name;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}