namespace QuizGame.Models
{
    public class Question
    {
        // Frågetexten som visas för spelaren
        public string Text { get; set; }

        // Index för det rätta svaret (0..2)
        public int CorrectAnswerIndex { get; set; }

        // Tre svarsalternativ (alltid 3 stycken)
        public string[] Answers { get; set; }

        // Kategori för frågan (t.ex. "Geografi", "Historia")
        public string Category { get; set; }

        // Sökväg till bild som hör till frågan (valfritt)
        public string ImagePath { get; set; }

        // Tom konstruktor – behövs för att läsa in från JSON
        public Question() { }

        // Konstruktor för att skapa ny fråga med tre svar
        public Question(string text, int correctIndex, string a1, string a2, string a3)
        {
            Text = text;
            Answers = new[] { a1, a2, a3 };
            CorrectAnswerIndex = Clamp(correctIndex);
        }

        // Konstruktor (extra version) – samma logik, tre svar
        public Question(string text, int correctIndex, string a1, string a2, string a3, string a4)
        {
            Text = text;
            Answers = new[] { a1, a2, a3 };
            CorrectAnswerIndex = Clamp(correctIndex);
        }

        // Kontrollerar om användarens val är rätt
        public bool IsCorrect(int index) => index == CorrectAnswerIndex;

        // Säkerställer att rätt index alltid är mellan 0 och 2
        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 2) return 2;
            return i;
        }
    }
}
