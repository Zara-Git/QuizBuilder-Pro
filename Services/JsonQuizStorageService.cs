using QuizGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;   // används för att läsa och spara JSON-data

namespace QuizGame.Services
{
    public class JsonQuizStorageService : IQuizStorageService
    {
        // Läser in alla quiz från JSON-filen (asynkront)
        public async Task<List<Quiz>> LoadAllAsync()
        {
            return await Task.Run(() =>
            {
                var file = AppPaths.QuizzesFile;
                if (!File.Exists(file))
                    return new List<Quiz>(); // om filen inte finns

                var json = File.ReadAllText(file);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Quiz>(); // om filen är tom

                var quizzes = JsonConvert.DeserializeObject<List<Quiz>>(json);
                return quizzes ?? new List<Quiz>(); // returnerar listan av quiz
            });
        }

        // Sparar alla quiz till JSON-filen (asynkront)
        public async Task SaveAllAsync(List<Quiz> quizzes)
        {
            await Task.Run(() =>
            {
                var file = AppPaths.QuizzesFile;

                // Skapar mappen om den inte finns
                var dir = Path.GetDirectoryName(file);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Skriver quizlistan till fil som JSON
                var json = JsonConvert.SerializeObject(quizzes, Formatting.Indented);
                File.WriteAllText(file, json);
            });
        }
    }
}
