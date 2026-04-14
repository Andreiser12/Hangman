using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HangmanGame.Data
{
    public class WordRepository
    {
        private readonly string _filePath;
        private Dictionary<string, List<string>> _words;

        public WordRepository()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "words.json");
            LoadWords();
        }

        private void LoadWords()
        {
            if (!File.Exists(_filePath))
            {
                _words = new Dictionary<string, List<string>>();
                return;
            }

            string json = File.ReadAllText(_filePath);
            _words = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                     ?? new Dictionary<string, List<string>>();
        }

        public List<string> GetCategories()
        {
            return _words.Keys.ToList();
        }

        public string GetRandomWord(string category)
        {
            var random = new Random();

            if (category == "All categories")
            {
                var allWords = _words.Values.SelectMany(list => list).ToList();
                return allWords[random.Next(allWords.Count)];
            }

            if (_words.ContainsKey(category))
            {
                var words = _words[category];
                return words[random.Next(words.Count)];
            }

            return string.Empty;
        }
    }
}
