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
                    OnPropertyChanged(nameof(ShowImageSection));
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

        private int _browsedAvatarIndex = -1;

        public ICommand BrowseImageCommand { get; }

        public NewUserViewModel()
        {
            _username = string.Empty;
            _imagePath = string.Empty;

            _avatars = new List<string>
            {
                "pack://application:,,,/HangmanGame;component/Resources/Avatars/avatar1.drawio.png",
                "pack://application:,,,/HangmanGame;component/Resources/Avatars/avatar2.drawio.png",
                "pack://application:,,,/HangmanGame;component/Resources/Avatars/avatar3.drawio.png"
            };

            BrowseImageCommand = new RelayCommand(_ => BrowseImage());
            PreviousAvatarCommand = new RelayCommand(_ => PreviousAvatar());
            NextAvatarCommand = new RelayCommand(_ => NextAvatar());

            NextAvatar();
        }

        private void NextAvatar()
        {
            if (_avatars.Count == 0) return;
            _currentAvatarIndex++;
            if (_currentAvatarIndex >= _avatars.Count) _currentAvatarIndex = 0;
            SelectAvatar();
        }

        private void PreviousAvatar()
        {
            if (_avatars.Count == 0) return;
            _currentAvatarIndex--;
            if (_currentAvatarIndex < 0) _currentAvatarIndex = _avatars.Count - 1;
            SelectAvatar();
        }

        private void SelectAvatar()
        {
            try
            {
                string uri = _avatars[_currentAvatarIndex];

                if (uri.StartsWith("file:///"))
                {
                    string absolutePath = new Uri(uri).LocalPath;
                    string relativePath = System.IO.Path.GetRelativePath(
                        AppDomain.CurrentDomain.BaseDirectory, absolutePath);
                    ImagePath = relativePath;
                }
                else
                {
                    string fileName = $"avatar_{DateTime.Now.Ticks}.png";
                    string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images");
                    if (!System.IO.Directory.Exists(imagesDir))
                        System.IO.Directory.CreateDirectory(imagesDir);

                    string destFile = System.IO.Path.Combine(imagesDir, fileName);

                    var resourceInfo = System.Windows.Application.GetResourceStream(new Uri(uri, UriKind.Absolute));
                    using (var fileStream = System.IO.File.Create(destFile))
                    {
                        resourceInfo.Stream.CopyTo(fileStream);
                    }

                    ImagePath = System.IO.Path.Combine("Data", "Images", fileName);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error");
            }
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
                if (!System.IO.Directory.Exists(imagesDir))
                {
                    System.IO.Directory.CreateDirectory(imagesDir);
                }
                string destFile = System.IO.Path.Combine(imagesDir, fileName);
                System.IO.File.Copy(sourceFile, destFile, overwrite: true);

                string relativePath = System.IO.Path.Combine("Data", "Images", fileName);
                ImagePath = relativePath;

                string fullUri = "file:///" + System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath).Replace("\\", "/");

                if (_browsedAvatarIndex >= 0)
                {
                    _avatars[_browsedAvatarIndex] = fullUri;
                }
                else
                {
                    _avatars.Add(fullUri);
                    _browsedAvatarIndex = _avatars.Count - 1;
                }

                _currentAvatarIndex = _browsedAvatarIndex;
            }
        }

        private List<string> _avatars;
        private int _currentAvatarIndex = -1;
        public ICommand PreviousAvatarCommand { get; }
        public ICommand NextAvatarCommand { get; }

        public bool ShowImageSection => !string.IsNullOrWhiteSpace(Username);
    }
}
