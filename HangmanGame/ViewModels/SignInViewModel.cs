using HangmanGame.Commands;
using HangmanGame.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HangmanGame.ViewModels
{
    public class SignInViewModel: ViewModelBase
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

        public ICommand PlayCommand { get; }
        public ICommand NewUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand CancelCommand { get; }

        public SignInViewModel()
        {
            Users = new ObservableCollection<User>();

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
        }

        private void Cancel()
        {
            
        }

        private void NewUser()
        {
            
        }

        private void DeleteUser()
        {
            
        }

        private void Play()
        {
            
        }
    }
}
