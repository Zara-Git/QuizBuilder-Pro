// Services/IQuizStorageService.cs
using QuizGame.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizGame.Services
{
    public interface IQuizStorageService
    {
        Task<List<Quiz>> LoadAllAsync();
        Task SaveAllAsync(List<Quiz> quizzes);
    }
}
