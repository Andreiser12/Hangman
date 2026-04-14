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

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            var clicked = sender as MenuItem;
            if (clicked == null) return;

            var vm = DataContext as ViewModels.GameViewModel;
            if (vm == null) return;

            var parent = clicked.Parent as MenuItem;
            if (parent != null)
            {
                foreach (var item in parent.Items)
                {
                    if (item is MenuItem menuItem && menuItem != clicked)
                        menuItem.IsChecked = false;
                }
            }

            clicked.IsChecked = true;
            vm.SelectedCategory = clicked.Tag.ToString();
        }
    }
}
