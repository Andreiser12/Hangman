using HangmanGame.Commands;
using HangmanGame.Data;
using HangmanGame.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HangmanGame.ViewModels
{
    public class OpenGameViewModel : ViewModelBase
    {
        private readonly SavedGameData _savedGameData;
        private readonly string _currentUsername;

        public ObservableCollection<SavedGame> SavedGames { get; set; }

        private SavedGame _selectedGame;
        public SavedGame SelectedGame
        {
            get => _selectedGame;
            set
            {
                if (_selectedGame != value)
                {
                    _selectedGame = value;
                    OnPropertyChanged(nameof(SelectedGame));
                }
            }
        }

        public ICommand DeleteGameCommand { get; }

        public OpenGameViewModel(string username)
        {
            _currentUsername = username;
            _savedGameData = new SavedGameData();

            var userGames = _savedGameData.LoadForUser(username);
            SavedGames = new ObservableCollection<SavedGame>(userGames);

            DeleteGameCommand = new RelayCommand(
                execute: _ => DeleteGame(),
                canExecute: _ => SelectedGame != null
            );
        }

        private void DeleteGame()
        {
            if (SelectedGame == null) return;

            _savedGameData.Delete(SelectedGame);
            SavedGames.Remove(SelectedGame);
            SelectedGame = null;
        }
    }
}