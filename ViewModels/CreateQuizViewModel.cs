using QuizGame.Models;
using QuizGame.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace QuizGame.ViewModels
{
    public class CreateQuizViewModel : INotifyPropertyChanged
    {
        private readonly IQuizStorageService _storage;

        // Lista över redan sparade quiz
        public ObservableCollection<Quiz> ExistingQuizzes { get; private set; }

        // Frågor som användaren håller på att skapa
        public ObservableCollection<Question> NewQuestions { get; private set; }

        // Namn på det nya quizet
        private string _quizName;
        public string QuizName
        {
            get => _quizName;
            set { _quizName = value; OnPropertyChanged(); }
        }

        // Text för den aktuella frågan
        private string _questionText;
        public string QuestionText
        {
            get => _questionText;
            set { _questionText = value; OnPropertyChanged(); }
        }

        // Tre svarsalternativ för frågan
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }

        // Extra fält för VG – kategori och bildsökväg
        public string Category { get; set; }
        public string ImagePath { get; set; }

        // Rätt svar (index 0–2)
        private int _correctIndex;
        public int CorrectIndex
        {
            get => _correctIndex;
            set
            {
                // Håller index inom intervallet 0–2
                if (value < 0) value = 0;
                if (value > 2) value = 2;
                _correctIndex = value;
                OnPropertyChanged();
            }
        }

        // Konstruktor – laddar befintliga quiz
        public CreateQuizViewModel(IQuizStorageService storage)
        {
            _storage = storage;
            ExistingQuizzes = new ObservableCollection<Quiz>();
            NewQuestions = new ObservableCollection<Question>();
            _ = LoadExistingAsync();
        }

        // Läser in sparade quiz från fil (asynkront)
        private async Task LoadExistingAsync()
        {
            var quizzes = await _storage.LoadAllAsync();
            ExistingQuizzes.Clear();
            foreach (var q in quizzes)
                ExistingQuizzes.Add(q);
        }

        // Lägger till ny fråga till det aktuella quizet
        public void AddQuestion()
        {
            // Kontrollerar att alla fält är ifyllda
            if (string.IsNullOrWhiteSpace(QuestionText) ||
                string.IsNullOrWhiteSpace(Answer1) ||
                string.IsNullOrWhiteSpace(Answer2) ||
                string.IsNullOrWhiteSpace(Answer3))
                return;

            // Skapar ny fråga med tre alternativ
            var q = new Question(QuestionText, CorrectIndex, Answer1, Answer2, Answer3)
            {
                Category = Category,
                ImagePath = ImagePath
            };

            // Lägger till frågan i listan
            NewQuestions.Add(q);

            // Rensar fälten efter att frågan lagts till
            QuestionText = string.Empty;
            Answer1 = Answer2 = Answer3 = string.Empty;
            Category = string.Empty;
            ImagePath = string.Empty;
            CorrectIndex = 0;

            OnPropertyChanged(nameof(QuestionText));
            OnPropertyChanged(nameof(Answer1));
            OnPropertyChanged(nameof(Answer2));
            OnPropertyChanged(nameof(Answer3));
            OnPropertyChanged(nameof(Category));
            OnPropertyChanged(nameof(ImagePath));
            OnPropertyChanged(nameof(CorrectIndex));
        }

        // Sparar hela quizet till fil (asynkront)
        public async Task SaveQuizAsync()
        {
            // Kontrollerar att quizet har namn och minst en fråga
            if (string.IsNullOrWhiteSpace(QuizName) || NewQuestions.Count == 0)
                return;

            // Läser in alla quiz som redan finns
            var all = await _storage.LoadAllAsync();

            // Skapar nytt quiz och lägger till frågor
            var newQuiz = new Quiz(QuizName);
            foreach (var q in NewQuestions)
                newQuiz.Questions.Add(q);

            // Lägger till nya quizet och sparar till fil
            all.Add(newQuiz);
            await _storage.SaveAllAsync(all);

            // Uppdaterar listan över existerande quiz
            await LoadExistingAsync();

            // Rensar formuläret
            QuizName = string.Empty;
            NewQuestions.Clear();
            OnPropertyChanged(nameof(QuizName));
        }

        // Hanterar ändringar i UI
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
