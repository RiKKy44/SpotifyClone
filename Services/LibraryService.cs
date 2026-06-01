using System.IO;
using System.Text.Json;
using GLab6.Models;

namespace GLab6.Services
{
    public static class LibraryService
    {
        private static string? _currentFilePath;
        public static string? CurrentFilePath => _currentFilePath;

        public static MusicLibrary Load(string filePath)
        {
            _currentFilePath = filePath;
            if (!File.Exists(filePath)) return new MusicLibrary();

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<MusicLibrary>(json) ?? new MusicLibrary();
        }

        public static void Save(MusicLibrary library)
        {
            if (string.IsNullOrEmpty(_currentFilePath)) return;

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(library, options);
            File.WriteAllText(_currentFilePath, json);
        }

        public static void CreateNew(string filePath)
        {
            _currentFilePath = filePath;
            Save(new MusicLibrary());
        }
    }
}