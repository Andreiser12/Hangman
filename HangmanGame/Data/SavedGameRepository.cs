using HangmanGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HangmanGame.Data
{
    public class SavedGameRepository
    {
        private readonly string _filePath;

        public SavedGameRepository()
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "saved_games.json");
        }

        public List<SavedGame> LoadAll()
        {
            if (!File.Exists(_filePath))
                return new List<SavedGame>();

            string json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<SavedGame>>(json) ?? new List<SavedGame>();
        }

        public List<SavedGame> LoadForUser(string username)
        {
            return LoadAll().Where(g => g.Username == username).ToList();
        }

        public void Save(SavedGame game)
        {
            var allGames = LoadAll();

            var existing = allGames.FirstOrDefault(
                g => g.Username == game.Username && g.GameName == game.GameName);
            if (existing != null)
                allGames.Remove(existing);

            allGames.Add(game);
            SaveAll(allGames);
        }

        public void Delete(SavedGame game)
        {
            var allGames = LoadAll();
            var toRemove = allGames.FirstOrDefault(
                g => g.Username == game.Username && g.GameName == game.GameName);
            if (toRemove != null)
            {
                allGames.Remove(toRemove);
                SaveAll(allGames);
            }
        }

        public void DeleteAllForUser(string username)
        {
            var allGames = LoadAll();
            allGames.RemoveAll(g => g.Username == username);
            SaveAll(allGames);
        }

        private void SaveAll(List<SavedGame> games)
        {
            string directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string json = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}