using HangmanGame.Commands;
using HangmanGame.Data;
using HangmanGame.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _currentGame.WordToGuess = _wordRepository.GetRandomWord(SelectedCategory);

            RemainingSeconds = 30;
            CurrentLevel = _currentGame.CurrentLevel;

            InitializeLetters();
            UpdateDisplayWord();
        }

        private void UpdateDisplayWord()
        {
            DisplayWord = string.Join(" ", _currentGame.WordToGuess
                .Select(c => _currentGame.GuessedLetters.Contains(c) ? c : '_'));
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
                    .All(c => _currentGame.GuessedLetters.Contains(c));

                if (wordComplete)
                {
                    _currentGame.CurrentLevel++;
                    CurrentLevel = _currentGame.CurrentLevel;

                    if (_currentGame.CurrentLevel >= 3)
                    {
                        
                    }
                    else
                    {
                        _currentGame.WordToGuess = _wordRepository.GetRandomWord(SelectedCategory);
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
    }
}
