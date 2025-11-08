// Models/Quiz.cs
using System.Collections.Generic;

namespace QuizGame.Models
{
    public class Quiz
    {
        // Namnet på quizet
        public string Name { get; set; }

        // Lista med alla frågor i quizet
        public List<Question> Questions { get; set; } = new List<Question>();

        // Tom konstruktor – behövs för att läsa från fil (JSON)
        public Quiz() { }

        // Skapar nytt quiz med ett namn
        public Quiz(string name)
        {
            Name = name;
        }
    }
}
