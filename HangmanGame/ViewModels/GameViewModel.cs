using HangmanGame.Commands;
using HangmanGame.Data;
using HangmanGame.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HangmanGame.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly WordRepository _wordRepository;
        private Game _currentGame;
        private User _currentUser;

        public string Username => _currentUser.Username;
        public string UserImagePath => _currentUser.ImagePath;

        private string _displayWord;
        public string DisplayWord
        {
            get => _displayWord;
            set
            {
                if (_displayWord != value)
                {
                    _displayWord = value;
                    OnPropertyChanged(nameof(DisplayWord));
                }
            }
        }

        private string _selectedCategory;
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));

                    _currentGame.CurrentLevel = 0;
                    CurrentLevel = 0;
                    NewGame();
                }
            }
        }

        private int _remainingSeconds;
        public int RemainingSeconds
        {
            get => _remainingSeconds;
            set
            {
                if (_remainingSeconds != value)
                {
                    _remainingSeconds = value;
                    OnPropertyChanged(nameof(RemainingSeconds));
                }
            }
        }

        private int _currentLevel;
        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                if (_currentLevel != value)
                {
                    _currentLevel = value;
                    OnPropertyChanged(nameof(CurrentLevel));
                }
            }
        }

        public ObservableCollection<string> Categories { get; set; }
        
        public ObservableCollection<LetterButton> Letters { get; set; }          
        public ICommand GuessLetterCommand { get; }

        private System.Windows.Threading.DispatcherTimer _timer;

        public GameViewModel(User user)
        {
            _currentUser = user;
            _wordRepository = new WordRepository();
            _currentGame = new Game();
            _displayWord = string.Empty;
            _selectedCategory = "All categories";

            var categoryList = _wordRepository.GetCategories();
            categoryList.Insert(0, "All categories");
            Categories = new ObservableCollection<string>(categoryList);
            Letters = new ObservableCollection<LetterButton>();
            InitializeLetters();
            GuessLetterCommand = new RelayCommand(
                execute: param => GuessLetter((char)param)
            );

            NewGame();
        }

        private void InitializeLetters()
        {
            Letters.Clear();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                Letters.Add(new LetterButton(c));
            }
        }

        public void NewGame()
        {
            _currentGame = new Game();
            _currentGame.Category = SelectedCategory;
            _currentGame.WordToGuess = _wordRepository.GetRandomWord(SelectedCategory).ToUpper();

            RemainingSeconds = 30;
            CurrentLevel = _currentGame.CurrentLevel;

            InitializeLetters();
            UpdateDisplayWord();
            StartTimer();
        }

        private void UpdateDisplayWord()
        {
            DisplayWord = string.Join(" ", _currentGame.WordToGuess
                .Select(c => c == ' ' || _currentGame.GuessedLetters.Contains(c) ? c : '_'));
        }

        private void GuessLetter(char letter)
        {
            var button = Letters.FirstOrDefault(l => l.Letter == letter);
            if (button != null)
                button.IsEnabled = false;

            if (_currentGame.WordToGuess.Contains(letter))
            {
                _currentGame.GuessedLetters.Add(letter);
                UpdateDisplayWord();

                bool wordComplete = _currentGame.WordToGuess
                    .All(c => c == ' ' || _currentGame.GuessedLetters.Contains(c));

                if (wordComplete)
                {
                    _currentGame.CurrentLevel++;
                    CurrentLevel = _currentGame.CurrentLevel;

                    if (_currentGame.CurrentLevel >= 3)
                    {
                        _timer?.Stop();
                    }
                    else
                    {
                        _currentGame.WordToGuess = _wordRepository.GetRandomWord(SelectedCategory).ToUpper();
                        _currentGame.GuessedLetters.Clear();
                        _currentGame.WrongLetters.Clear();
                        RemainingSeconds = 30;
                        InitializeLetters();
                        UpdateDisplayWord();
                    }
                }
            }
            else
            {
                _currentGame.WrongLetters.Add(letter);
            }
        }

        private void StartTimer()
        {
            _timer?.Stop();

            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, e) =>
            {
                RemainingSeconds--;

                if (RemainingSeconds <= 0)
                {
                    _timer.Stop();
                    LoseRound();
                }
            };
            _timer.Start();
        }

        private void LoseRound()
        {
            _timer?.Stop();
            System.Windows.MessageBox.Show(
                $"Time's up! The word was: {_currentGame.WordToGuess}",
                "Round Lost");

            _currentGame.CurrentLevel = 0;
            CurrentLevel = 0;

            NewGame();
        }
    }
}
