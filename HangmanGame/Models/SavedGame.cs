using System.Collections.Generic;

namespace HangmanGame.Models
{
    public class SavedGame
    {
        public string GameName { get; set; }
        public string Username { get; set; }
        public string Category { get; set; }
        public string WordToGuess { get; set; }
        public List<char> GuessedLetters { get; set; }
        public List<char> WrongLetters { get; set; }
        public int CurrentLevel { get; set; }
        public int RemainingSeconds { get; set; }
        public List<string> UsedWords { get; set; }

        public SavedGame()
        {
            GameName = string.Empty;
            Username = string.Empty;
            Category = string.Empty;
            WordToGuess = string.Empty;
            GuessedLetters = new List<char>();
            WrongLetters = new List<char>();
            UsedWords = new List<string>();
        }
    }
}