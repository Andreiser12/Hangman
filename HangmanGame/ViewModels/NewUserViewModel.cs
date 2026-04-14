using HangmanGame.Commands;
using System.Windows.Input;

namespace HangmanGame.ViewModels
{
    public class NewUserViewModel: ViewModelBase
    {
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        private string _imagePath = string.Empty;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public ICommand BrowseImageCommand { get; }

        public NewUserViewModel()
        {
            BrowseImageCommand = new RelayCommand(
                execute: _ => BrowseImage()
            );
        }

        private void BrowseImage()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image Files (*.jpg;*.gif)|*.jpg;*.gif";

            if (dialog.ShowDialog() == true)
            {
                string sourceFile = dialog.FileName;
                string fileName = System.IO.Path.GetFileName(sourceFile);
                string imagesDir = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Data", "Images");

                if(!System.IO.Directory.Exists(imagesDir))
                {
                    System.IO.Directory.CreateDirectory(imagesDir);
                }

                string destFile = System.IO.Path.Combine(
                    imagesDir, fileName);
                System.IO.File.Copy(sourceFile, destFile, overwrite: true);

                ImagePath = System.IO.Path.Combine("Data", "Images", fileName);
            }
        }
    }
}
