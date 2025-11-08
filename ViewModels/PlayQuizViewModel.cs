using QuizGame.Models;
using QuizGame.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace QuizGame.ViewModels
{
    public class PlayQuizViewModel : INotifyPropertyChanged
    {
        private readonly IQuizStorageService _storage;
        private readonly Random _random = new Random();

        // Hur många frågor som spelas per runda
        private const int TargetQuestionsPerRun = 10;

        // Lista över alla quiz
        public ObservableCollection<Quiz> Quizzes { get; private set; }

        private Quiz _selectedQuiz;
        public Quiz SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                if (_selectedQuiz != value)
                {
                    _selectedQuiz = value;
                    OnPropertyChanged();
                    StartNewRun(); // Startar ny omgång när quiz väljs
                }
            }
        }

        // Den aktuella frågan som visas
        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            private set { _currentQuestion = value; OnPropertyChanged(); }
        }

        // Interna speldata
        private List<Question> _pool = new List<Question>(); // Frågepool för rundan
        private int _startPoolCount = 0;
        private int _runCorrect = 0;     // Antal rätt i nuvarande runda
        private int _totalAnswered = 0;  // Totalt antal besvarade
        private int _totalCorrect = 0;   // Totalt antal rätt

        // Text som visar poäng (t.ex. "5 / 7 (71%)")
        public string ScoreText
        {
            get
            {
                if (_totalAnswered == 0) return "0 / 0 (0%)";
                int p = (int)((double)_totalCorrect / _totalAnswered * 100);
                return $"{_totalCorrect} / {_totalAnswered} ({p}%)";
            }
        }

        private bool _hasFailed;
        public bool HasFailed
        {
            get => _hasFailed;
            private set { _hasFailed = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsGameOver)); }
        }

        private bool _hasWon;
        public bool HasWon
        {
            get => _hasWon;
            private set { _hasWon = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsGameOver)); }
        }

        // Sant om rundan är slut (vinst eller förlust)
        public bool IsGameOver => HasFailed || HasWon;

        private string _resultText = "";
        public string ResultText
        {
            get => _resultText;
            private set { _resultText = value; OnPropertyChanged(); }
        }

        public PlayQuizViewModel(IQuizStorageService storage)
        {
            _storage = storage;
            Quizzes = new ObservableCollection<Quiz>();
            _ = LoadQuizzesAsync();
        }

        // Läser quiz från fil (eller skapar standard om ingen finns)
        private async Task LoadQuizzesAsync()
        {
            var quizzes = await _storage.LoadAllAsync();
            if (quizzes == null || quizzes.Count == 0)
            {
                quizzes = CreateDefaultQuizzes(); // Skapar två standardquiz
                await _storage.SaveAllAsync(quizzes);
            }

            Quizzes.Clear();
            foreach (var q in quizzes) Quizzes.Add(q);
            SelectedQuiz = Quizzes.FirstOrDefault();
        }

        // Hanterar svar på frågan
        public void Answer(int answerIndex)
        {
            if (CurrentQuestion == null || IsGameOver) return;

            _totalAnswered++;

            // Om svaret är rätt
            if (CurrentQuestion.IsCorrect(answerIndex))
            {
                _totalCorrect++;
                _runCorrect++;
                _pool.Remove(CurrentQuestion);

                // Kollar om spelaren vunnit rundan
                if (_runCorrect >= Math.Min(TargetQuestionsPerRun, _startPoolCount) || _pool.Count == 0)
                {
                    HasWon = true;
                    ResultText = $"Bra! Du klarade {_runCorrect} av {Math.Min(TargetQuestionsPerRun, _startPoolCount)}.";
                    CurrentQuestion = null;
                }
                else
                {
                    LoadRandomQuestion(); // Laddar nästa fråga
                }
            }
            else
            {
                // Om svaret är fel
                HasFailed = true;
                ResultText = "Fel svar! Rätt svar var: " +
                             CurrentQuestion.Answers[CurrentQuestion.CorrectAnswerIndex] + ".\n" +
                             $"Du fick {_runCorrect} rätt av {Math.Min(TargetQuestionsPerRun, _startPoolCount)} den här rundan.";
                CurrentQuestion = null;
            }

            OnPropertyChanged(nameof(ScoreText));
        }

        // Startar om spelet
        public void Restart() => StartNewRun();

        // Förbereder en ny runda (blandar frågor)
        private void StartNewRun()
        {
            _runCorrect = 0;
            HasFailed = false;
            HasWon = false;
            ResultText = "";

            if (SelectedQuiz == null || SelectedQuiz.Questions == null || SelectedQuiz.Questions.Count == 0)
            {
                _pool = new List<Question>();
                CurrentQuestion = null;
                return;
            }

            // Blandar frågor slumpmässigt
            _pool = SelectedQuiz.Questions.OrderBy(_ => _random.Next()).ToList();
            _startPoolCount = Math.Min(TargetQuestionsPerRun, _pool.Count);
            if (_pool.Count > _startPoolCount)
                _pool = _pool.Take(_startPoolCount).ToList();

            LoadRandomQuestion();
            OnPropertyChanged(nameof(ScoreText));
        }

        // Hämtar nästa fråga från poolen
        private void LoadRandomQuestion()
        {
            if (_pool.Count == 0) { CurrentQuestion = null; return; }
            CurrentQuestion = _pool[_random.Next(_pool.Count)];
        }

        // ===== Standarddata (skapas första gången appen körs) =====
        private List<Quiz> CreateDefaultQuizzes()
        {
            var cities = new Quiz("Swedish Cities");
            cities.Questions.Add(new Question("Capital of Sweden?", 0, "Stockholm", "Göteborg", "Malmö", "Uppsala"));
            cities.Questions.Add(new Question("Northernmost large city?", 2, "Umeå", "Östersund", "Luleå", "Gävle"));
            cities.Questions.Add(new Question("Where is Turning Torso?", 3, "Uppsala", "Göteborg", "Stockholm", "Malmö"));
            cities.Questions.Add(new Question("Island city with ringmur?", 1, "Kalmar", "Visby", "Karlskrona", "Norrköping"));
            cities.Questions.Add(new Question("City famous for Gothia Cup?", 0, "Göteborg", "Stockholm", "Helsingborg", "Malmö"));
            cities.Questions.Add(new Question("University town in Skåne?", 2, "Umeå", "Luleå", "Lund", "Karlstad"));
            cities.Questions.Add(new Question("Öresund Bridge connects via…", 3, "Helsingborg", "Halmstad", "Kalmar", "Malmö"));
            cities.Questions.Add(new Question("Icehotel is near…", 1, "Umeå", "Kiruna", "Skellefteå", "Hudiksvall"));
            cities.Questions.Add(new Question("Largest lake by Karlstad?", 0, "Vänern", "Vättern", "Mälaren", "Hjälmaren"));
            cities.Questions.Add(new Question("Ferry hub to Finland up north?", 2, "Sundsvall", "Örnsköldsvik", "Haparanda/Tornio", "Falun"));

            var vikings = new Quiz("Vikings & History");
            vikings.Questions.Add(new Question("Old name for Sweden in runestones?", 3, "Svearike", "Svitjod", "Svealand", "Svíþjóð"));
            vikings.Questions.Add(new Question("Viking trading city Birka is in…", 1, "Göteborg archipelago", "Lake Mälaren", "Öresund", "Götakanal"));
            vikings.Questions.Add(new Question("Oseberg ship belonged to…", 0, "Norway", "Denmark", "Iceland", "Sweden"));
            vikings.Questions.Add(new Question("Runic alphabet is called…", 2, "Latin", "Cyrillic", "Futhark", "Ogham"));
            vikings.Questions.Add(new Question("Vikings reached as far as…", 3, "Greenland only", "Iceland only", "North Africa only", "North America"));
            vikings.Questions.Add(new Question("Leif Erikson is associated with…", 1, "Greenland settlement", "Vinland voyages", "Norman conquest", "Crusades"));
            vikings.Questions.Add(new Question("Varangians served as guards in…", 0, "Byzantine Empire", "Holy Roman Empire", "Mongol Empire", "Persian Empire"));
            vikings.Questions.Add(new Question("Uppsala famous for ancient…", 2, "Cathedral bells", "Canals", "Mounds/temple site", "Fortresses"));
            vikings.Questions.Add(new Question("Most runestones are found in…", 3, "Norway", "Denmark", "Iceland", "Sweden"));
            vikings.Questions.Add(new Question("Term for Viking explorers eastwards:", 1, "Skraelings", "Rus", "Normans", "Saxons"));

            return new List<Quiz> { cities, vikings };
        }

        // Notifierar UI om förändringar
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
