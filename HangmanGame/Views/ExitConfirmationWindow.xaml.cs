using System.Windows;

namespace HangmanGame.Views
{
    public enum ExitChoice
    {
        Save,
        DontSave,
        Cancel
    }

    public partial class ExitConfirmationWindow : Window
    {
        public ExitChoice Choice { get; private set; } = ExitChoice.Cancel;

        public ExitConfirmationWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Choice = ExitChoice.Save;
            DialogResult = true;
        }

        private void DontSave_Click(object sender, RoutedEventArgs e)
        {
            Choice = ExitChoice.DontSave;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Choice = ExitChoice.Cancel;
            DialogResult = false;
        }
    }
}