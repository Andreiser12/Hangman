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
        #region Constructor & Base Dependencies
        private readonly WordData _wordData;
        private readonly SavedGameData _savedGameData;
        private readonly StatisticsData _statisticsData;

        public GameViewModel(User user)
        {
            _currentUser = user;
            _wordData = new WordData();
            _currentGame = new Game();
            _displayWord = string.Empty;
            _selectedCategory = "All categories";
            _savedGameData = new SavedGameData();
            _statisticsData = new StatisticsData();

            var categoryList = _wordData.GetCategories();
            categoryList.Insert(0, "All categories");
            Categories = new ObservableCollection<string>(categoryList);
            Letters = new ObservableCollection<LetterButton>();
            InitializeLetters();
            GuessLetterCommand = new RelayCommand(
                execute: param => GuessLetter((char)param)
            );
            
            SaveGameCommand = new RelayCommand(
                execute: _ => SaveGame()
            );

            OpenGameCommand = new RelayCommand(
                execute: _ => OpenGame()
            );

            StatisticsCommand = new RelayCommand(
                execute: _ => ShowStatistics()
            );

            CancelCommand = new RelayCommand(
                execute: _ => Cancel()
            );

            NewGame();
        }
        #endregion

        #region User Info
        private User _currentUser;
        public string Username => _currentUser.Username;
        public string UserImagePath => _currentUser.ImagePath;
        #endregion

        #region Game State & Word Interaction
        private Game _currentGame;
        private List<string> _usedWords = new List<string>();
        private readonly int MAX_MISTAKES = 9;

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

        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<LetterButton> Letters { get; set; }

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

        public ICommand GuessLetterCommand { get; }

        public void NewGame()
        {
            _currentGame = new Game();
            _currentGame.Category = SelectedCategory;
            _usedWords.Clear();
            _currentGame.WordToGuess = GetUniqueWord();

            RemainingSeconds = 12;
            CurrentLevel = _currentGame.CurrentLevel;

            DisplayWordColor = "White";

            InitializeLetters();
            UpdateDisplayWord();
            StartTimer();
            UpdateHangmanImage();
        }

        private void InitializeLetters()
        {
            Letters.Clear();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                Letters.Add(new LetterButton(c));
            }
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

                if (_currentGame.WrongLetters.Count >= MAX_MISTAKES)
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
                word = _wordData.GetRandomWord(SelectedCategory).ToUpper();
                attempts++;
                if (attempts > 100) break;
            } while (_usedWords.Contains(word));

            _usedWords.Add(word);
            return word;
        }

        private void UpdateHangmanImage()
        {
            int wrongCount = _currentGame.WrongLetters.Count;
            if (wrongCount > MAX_MISTAKES) wrongCount = MAX_MISTAKES;

            HangmanImagePath = $"pack://application:,,,/Resources/HangmanDisplayed/try{wrongCount}.drawio.png";
        }
        #endregion

        #region Timer Logic
        private DispatcherTimer _timer;
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
        #endregion

        #region Game Over & Application Navigation
        public ICommand CancelCommand { get; }

        private void ShowGameOver(GameResult result)
        {
            _timer?.Stop();

            bool hasWon = result == GameResult.Won;
            _statisticsData.RecordGame(_currentUser.Username, _currentGame.Category, hasWon);

            string word = _currentGame.WordToGuess;
            var vm = new GameResultViewModel(result, word);
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
                GoToSignIn();
            }
        }

        private void Cancel()
        {
            _timer?.Stop();

            var dialog = new Views.ExitConfirmationWindow();
            dialog.ShowDialog();

            if (dialog.Choice == Views.ExitChoice.Cancel)
            {
                StartTimer();
                return;
            }

            if (dialog.Choice == Views.ExitChoice.Save)
            {
                SaveGame();
            }

            GoToSignIn();
        }

        public bool IsExiting { get; set; } = false;

        private void GoToSignIn()
        {
            IsExiting = true;
            _timer?.Stop();
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
        #endregion

        #region Save / Open Logic
        public ICommand SaveGameCommand { get; }
        public ICommand OpenGameCommand { get; }

        private void SaveGame()
        {
            _timer?.Stop();

            var dialog = new Views.SaveGameWindow();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                var savedGame = new SavedGame
                {
                    GameName = dialog.GameName,
                    Username = _currentUser.Username,
                    Category = _currentGame.Category,
                    WordToGuess = _currentGame.WordToGuess,
                    GuessedLetters = new List<char>(_currentGame.GuessedLetters),
                    WrongLetters = new List<char>(_currentGame.WrongLetters),
                    CurrentLevel = _currentGame.CurrentLevel,
                    RemainingSeconds = RemainingSeconds,
                    UsedWords = new List<string>(_usedWords)
                };

                _savedGameData.Save(savedGame);

                System.Windows.MessageBox.Show(
                    $"Game '{dialog.GameName}' saved successfully!",
                    "Save Game",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }

            StartTimer();
        }

        private void OpenGame()
        {
            _timer?.Stop();

            var dialog = new Views.OpenGameWindow();
            dialog.DataContext = new OpenGameViewModel(_currentUser.Username);
            bool? result = dialog.ShowDialog();

            if (result == true && dialog.SelectedGame != null)
            {
                var saved = dialog.SelectedGame;

                _currentGame = new Game();
                _currentGame.Category = saved.Category;
                _currentGame.WordToGuess = saved.WordToGuess;
                _currentGame.GuessedLetters = new List<char>(saved.GuessedLetters);
                _currentGame.WrongLetters = new List<char>(saved.WrongLetters);
                _currentGame.CurrentLevel = saved.CurrentLevel;

                _usedWords = new List<string>(saved.UsedWords);

                _selectedCategory = saved.Category;
                OnPropertyChanged(nameof(SelectedCategory));

                CurrentLevel = saved.CurrentLevel;
                RemainingSeconds = saved.RemainingSeconds;

                InitializeLetters();
                foreach (char letter in saved.GuessedLetters.Concat(saved.WrongLetters))
                {
                    var btn = Letters.FirstOrDefault(l => l.Letter == letter);
                    if (btn != null) btn.IsEnabled = false;
                }

                UpdateDisplayWord();
                UpdateHangmanImage();
                StartTimer();
            }
            else
            {
                StartTimer();
            }
        }
        #endregion

        #region Statistics Logic
        public ICommand StatisticsCommand { get; }

        private void ShowStatistics()
        {
            _timer?.Stop();

            var window = new Views.StatisticsWindow();
            window.DataContext = new StatisticsViewModel();
            window.ShowDialog();

            StartTimer();
        }
        #endregion
    }
}
