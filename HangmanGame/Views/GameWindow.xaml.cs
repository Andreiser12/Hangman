using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HangmanGame.Views
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModels.GameViewModel;
            vm?.NewGame();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Nume: Șerbănescu Andrei\nGrupa: 10LF244\nSpecializarea: Informatică",
                            "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
