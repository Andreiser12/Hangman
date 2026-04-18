using HangmanGame.Commands;
using HangmanGame.Data;
using HangmanGame.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace HangmanGame.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly WordRepository _wordRepository;
        private Game _currentGame;
        private User _currentUser;
        private List<string> _usedWords = new List<string>();

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

        private string _hangmanImagePath;
        public string HangmanImagePath
        {
            get => _hangmanImagePath;
            set
            {
                if (_hangmanImagePath != value)
                {
                    _hangmanImagePath = value;
                    OnPropertyChanged(nameof(HangmanImagePath));
                }
            }
        }

        private string _displayWordColor = "White";
        public string DisplayWordColor
        {
            get => _displayWordColor;
            set
            {
                if (_displayWordColor != value)
                {
                    _displayWordColor = value;
                    OnPropertyChanged(nameof(DisplayWordColor));
                }
            }
        }

        public ObservableCollection<string> Categories { get; set; }
        
        public ObservableCollection<LetterButton> Letters { get; set; }          
        public ICommand GuessLetterCommand { get; }

        private DispatcherTimer _timer;

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
            _usedWords.Clear();

            RemainingSeconds = 12;
            CurrentLevel = _currentGame.CurrentLevel;

            InitializeLetters();
            UpdateDisplayWord();
            StartTimer();
            UpdateHangmanImage();
        }

        private void UpdateDisplayWord()
        {
            DisplayWord = string.Join(" ", _currentGame.WordToGuess
                .Select(c => c == ' ' || _currentGame.GuessedLetters.Contains(c) ? c : '_'));
        }

        private void GuessLetter(char letter)
        {
            var button = Letters.FirstOrDefault(l => l.Letter == letter);
            if (button == null || !button.IsEnabled)
                return;

            button.IsEnabled = false;

            if (_currentGame.WordToGuess.Contains(letter))
            {
                _currentGame.GuessedLetters.Add(letter);
                RemainingSeconds = 12;
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
                        DisplayWordColor = "LimeGreen";

                        var winTimer = new DispatcherTimer();
                        winTimer.Interval = TimeSpan.FromSeconds(2);
                        winTimer.Tick += (s, ev) =>
                        {
                            winTimer.Stop();
                            ShowGameOver(GameResult.Won);
                        };
                        winTimer.Start();
                    }
                    else
                    {
                        _timer?.Stop();
                        DisplayWordColor = "LimeGreen";

                        var delayTimer = new DispatcherTimer();
                        delayTimer.Interval = TimeSpan.FromSeconds(2);
                        delayTimer.Tick += (s, ev) =>
                        {
                            delayTimer.Stop();
                            DisplayWordColor = "White";

                            _currentGame.WordToGuess = GetUniqueWord();
                            _currentGame.GuessedLetters.Clear();
                            _currentGame.WrongLetters.Clear();
                            RemainingSeconds = 12;
                            InitializeLetters();
                            UpdateDisplayWord();
                            UpdateHangmanImage();
                            StartTimer();
                        };
                        delayTimer.Start();
                    }
                }
            }
            else
            {
                _currentGame.WrongLetters.Add(letter);
                UpdateHangmanImage();

                if (_currentGame.WrongLetters.Count >= 9)
                {
                    ShowGameOver(GameResult.LostTooManyTries);
                }
            }
        }

        private string GetUniqueWord()
        {
            string word;
            int attempts = 0;

            do
            {
                word = _wordRepository.GetRandomWord(SelectedCategory).ToUpper();
                attempts++;
                if (attempts > 100) break;
            } while (_usedWords.Contains(word));

            _usedWords.Add(word);
            return word;
        }

        private void StartTimer()
        {
            _timer?.Stop();

            _timer = new DispatcherTimer();
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
            ShowGameOver(GameResult.LostTimeUp);
        }

        private void UpdateHangmanImage()
        {
            int wrongCount = _currentGame.WrongLetters.Count;
            if (wrongCount > 9) wrongCount = 9;

            HangmanImagePath = $"pack://application:,,,/Resources/HangmanDisplayed/try{wrongCount}.drawio.png";
        }

        private void ShowGameOver(GameResult result)
        {
            _timer?.Stop();

            string word = _currentGame.WordToGuess;
            var vm = new GameOverViewModel(result, word);
            var window = new Views.GameOverWindow();
            window.DataContext = vm;
            window.ShowDialog();

            if (vm.PlayAgainClicked)
            {
                _currentGame.CurrentLevel = 0;
                CurrentLevel = 0;
                NewGame();
            }
            else
            {
                var signInWindow = new Views.SignInWindow();
                System.Windows.Application.Current.MainWindow = signInWindow;
                signInWindow.Show();

                foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
                {
                    if (w is Views.GameWindow)
                    {
                        w.Close();
                        break;
                    }
                }
            }
        }
    }
}
