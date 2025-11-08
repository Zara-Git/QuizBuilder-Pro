using QuizGame.Models;
using QuizGame.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace QuizGame.ViewModels
{
    public class EditQuizViewModel : INotifyPropertyChanged
    {
        private readonly IQuizStorageService _storage;

        public ObservableCollection<Quiz> Quizzes { get; } = new ObservableCollection<Quiz>();

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
                    SelectedQuestion = _selectedQuiz?.Questions?.FirstOrDefault();
                    OnPropertyChanged(nameof(Questions));
                }
            }
        }

       
        public ObservableCollection<Question> Questions
            => new ObservableCollection<Question>(_selectedQuiz?.Questions ?? Enumerable.Empty<Question>());

        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                if (_selectedQuestion != value)
                {
                    _selectedQuestion = value;
                    OnPropertyChanged();
                    LoadFieldsFromSelected();
                }
            }
        }

        
        private string _editText;
        public string EditText { get => _editText; set { _editText = value; OnPropertyChanged(); } }

        private string _a1, _a2, _a3;
        public string Answer1 { get => _a1; set { _a1 = value; OnPropertyChanged(); } }
        public string Answer2 { get => _a2; set { _a2 = value; OnPropertyChanged(); } }
        public string Answer3 { get => _a3; set { _a3 = value; OnPropertyChanged(); } }

        private int _correctIndex;
        public int CorrectIndex
        {
            get => _correctIndex;
            set
            {
                if (value < 0) value = 0;
                if (value > 2) value = 2;
                _correctIndex = value;
                OnPropertyChanged();
            }
        }

        public EditQuizViewModel(IQuizStorageService storage)
        {
            _storage = storage;
            _ = LoadQuizzesAsync();
        }

        private async Task LoadQuizzesAsync()
        {
            var all = await _storage.LoadAllAsync();
            Quizzes.Clear();
            foreach (var q in all) Quizzes.Add(q);
            SelectedQuiz = Quizzes.FirstOrDefault();
        }

        private void LoadFieldsFromSelected()
        {
            if (SelectedQuestion == null)
            {
                EditText = "";
                Answer1 = Answer2 = Answer3 = "";
                CorrectIndex = 0;
                return;
            }

            EditText = SelectedQuestion.Text;
            var arr = SelectedQuestion.Answers ?? new string[3];
            Answer1 = arr.Length > 0 ? arr[0] : "";
            Answer2 = arr.Length > 1 ? arr[1] : "";
            Answer3 = arr.Length > 2 ? arr[2] : "";

            var idx = SelectedQuestion.CorrectAnswerIndex;
            CorrectIndex = idx < 0 ? 0 : idx > 2 ? 2 : idx;
        }

       
     
        public bool ApplyChangesToSelected()
        {
            if (SelectedQuiz == null || SelectedQuestion == null) return false;

            var t = EditText?.Trim();
            var a1 = Answer1?.Trim();
            var a2 = Answer2?.Trim();
            var a3 = Answer3?.Trim();

            if (string.IsNullOrEmpty(t) || string.IsNullOrEmpty(a1) ||
                string.IsNullOrEmpty(a2) || string.IsNullOrEmpty(a3))
                return false;

            var idx = CorrectIndex;
            if (idx < 0) idx = 0;
            if (idx > 2) idx = 2;

            SelectedQuestion.Text = t;
            SelectedQuestion.Answers = new[] { a1, a2, a3 };
            SelectedQuestion.CorrectAnswerIndex = idx;

          
            OnPropertyChanged(nameof(Questions));
            return true;
        }

        
        public void ApplyEditsToSelected() => ApplyChangesToSelected();

        public void AddNewQuestion()
        {
            if (SelectedQuiz == null) return;

            var q = new Question("New question", 0, "A1", "A2", "A3");
            SelectedQuiz.Questions.Add(q);
            SelectedQuestion = q;
            OnPropertyChanged(nameof(Questions));
        }

        public void DeleteSelectedQuestion()
        {
            if (SelectedQuiz == null || SelectedQuestion == null) return;

            SelectedQuiz.Questions.Remove(SelectedQuestion);
            SelectedQuestion = SelectedQuiz.Questions.FirstOrDefault();
            OnPropertyChanged(nameof(Questions));
        }

        public async Task SaveAllAsync()
        {
            await _storage.SaveAllAsync(Quizzes.ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
