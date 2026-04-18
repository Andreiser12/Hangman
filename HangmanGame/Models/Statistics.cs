namespace HangmanGame.Models
{
    public class Statistics
    {
        public string Username { get; set; }
        public string Category { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }

        public Statistics()
        {
            Username = string.Empty;
            Category = string.Empty;
            GamesPlayed = 0;
            GamesWon = 0;
        }
    }
}