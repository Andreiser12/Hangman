using HangmanGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HangmanGame.Data
{
    public class StatisticsData
    {
        private readonly string _filePath;

        public StatisticsData()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "statistics.json");
        }

        public List<Statistics> LoadAll()
        {
            if (!File.Exists(_filePath))
                return new List<Statistics>();

            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Statistics>>(json) ?? new List<Statistics>();
        }

        public void RecordGame(string username, string category, bool hasWon)
        {
            var allStats = LoadAll();

            var entry = allStats.FirstOrDefault(
                s => s.Username == username && s.Category == category);

            if (entry == null)
            {
                entry = new Statistics
                {
                    Username = username,
                    Category = category,
                    GamesPlayed = 0,
                    GamesWon = 0
                };
                allStats.Add(entry);
            }

            entry.GamesPlayed++;
            if (hasWon)
                entry.GamesWon++;

            SaveAll(allStats);
        }

        public void DeleteAllForUser(string username)
        {
            var allStats = LoadAll();
            allStats.RemoveAll(s => s.Username == username);
            SaveAll(allStats);
        }

        private void SaveAll(List<Statistics> stats)
        {
            string directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}