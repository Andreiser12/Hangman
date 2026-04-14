using System.Collections.Generic;


namespace HangmanGame.Models
{
    public class Game
    {
        public string WordToGuess { get; set; }
        public List<char> GuessedLetters { get; set; }
        public List<char> WrongLetters { get; set; }
        public string Category { get; set; }
        public int CurrentLevel { get; set; }
        public int RemainingSeconds { get; set; }

        public Game()
        {
            WordToGuess = string.Empty;
            GuessedLetters = new List<char>();
            WrongLetters = new List<char>();
            Category = "All categories";
            CurrentLevel = 0;
            RemainingSeconds = 30;
        }
    }
}
