using HangmanGame.Commands;
using HangmanGame.Data;
using HangmanGame.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace HangmanGame.ViewModels
{
    public class SignInViewModel : ViewModelBase
    {
        public ObservableCollection<User> Users { get; set; }

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    OnPropertyChanged(nameof(SelectedUser));
                }
            }
        }

        private readonly UserRepository _userRepository;

        public ICommand PlayCommand { get; }
        public ICommand NewUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PreviousUserCommand { get; }
        public ICommand NextUserCommand { get; }

        public SignInViewModel()
        {
            _userRepository = new UserRepository();
            Users = new ObservableCollection<User>(_userRepository.LoadUsers());

            PlayCommand = new RelayCommand(
                execute: _ => Play(),
                canExecute: _ => SelectedUser != null
            );

            DeleteUserCommand = new RelayCommand(
                execute: _ => DeleteUser(),
                canExecute: _ => SelectedUser != null
            );

            NewUserCommand = new RelayCommand(
                execute: _ => NewUser()
            );

            CancelCommand = new RelayCommand(
                execute: _ => Cancel()
            );

            PreviousUserCommand = new RelayCommand(
                execute: _ => PreviousUser(),
                canExecute: _ => Users.Count > 0
            );
            NextUserCommand = new RelayCommand(
                execute: _ => NextUser(),
                canExecute: _ => Users.Count > 0
            );
        }

        private void PreviousUser()
        {
            if (Users.Count == 0) return;

            int currentIndex = SelectedUser != null ? Users.IndexOf(SelectedUser) : 0;
            currentIndex--;
            if (currentIndex < 0) currentIndex = Users.Count - 1;

            SelectedUser = Users[currentIndex];
        }

        private void NextUser()
        {
            if (Users.Count == 0) return;

            int currentIndex = SelectedUser != null ? Users.IndexOf(SelectedUser) : -1;
            currentIndex++;
            if (currentIndex >= Users.Count) currentIndex = 0;

            SelectedUser = Users[currentIndex];
        }

        private void Cancel()
        {
            Application.Current.Shutdown();
        }

        private void NewUser()
        {
            var dialog = new Views.NewUserWindow();

            if (dialog.ShowDialog() == true)
            {
                var vm = dialog.DataContext as NewUserViewModel;

                if (!string.IsNullOrWhiteSpace(vm.Username))
                {
                    var newUser = new User
                    {
                        Username = vm.Username,
                        ImagePath = vm.ImagePath
                    };
                    Users.Add(newUser);
                    _userRepository.SaveUsers(Users.ToList());
                }
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                SelectedUser = null;
                _userRepository.SaveUsers(Users.ToList());
            }
        }

        private void Play()
        {
            if (SelectedUser == null) return;

            var gameWindow = new Views.GameWindow();
            gameWindow.DataContext = new GameViewModel(SelectedUser);

            var previousWindow = Application.Current.MainWindow;
            Application.Current.MainWindow = gameWindow;
            gameWindow.Show();
            previousWindow?.Close();
        }
    }
}
