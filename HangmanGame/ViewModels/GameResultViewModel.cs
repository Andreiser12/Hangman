using HangmanGame.Commands;
using System.Windows.Input;

namespace HangmanGame.ViewModels
{
    public enum GameResult
    {
        Won,
        LostTooManyTries,
        LostTimeUp
    }

    public class GameResultViewModel : ViewModelBase
    {
        public string Message { get; set; }
        public ICommand PlayAgainCommand { get; }
        public ICommand BackToSignInCommand { get; }
        public bool PlayAgainClicked { get; private set; }

        public GameResultViewModel(GameResult result, string word = null)
        {
            Message = result switch
            {
                GameResult.Won => "You won!",
                GameResult.LostTooManyTries => $"You lost! Too many tries.\nThe word was:\n {word}",
                GameResult.LostTimeUp => $"You lost! Time's up.\nThe word was:\n {word}",
                _ => ""
            };

            PlayAgainCommand = new RelayCommand(_ => PlayAgain());
            BackToSignInCommand = new RelayCommand(_ => BackToSignIn());
        }

        private void PlayAgain()
        {
            PlayAgainClicked = true;
            CloseWindow();
        }

        private void BackToSignIn()
        {
            PlayAgainClicked = false;
            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (var window in System.Windows.Application.Current.Windows)
            {
                if (window is Views.GameOverWindow w)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}