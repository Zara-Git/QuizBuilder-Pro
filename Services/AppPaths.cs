// Services/AppPaths.cs
using System.IO;

namespace QuizGame.Services
{
    public static class AppPaths
    {
        public static string GetAppFolder()
        {
            var root = System.Environment.GetFolderPath(
                System.Environment.SpecialFolder.LocalApplicationData);
            var folder = Path.Combine(root, "QuizGame");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }

        public static string QuizzesFile =>
            Path.Combine(GetAppFolder(), "quizzes.json");
    }
}
