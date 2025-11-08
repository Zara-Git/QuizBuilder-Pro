using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuizGame.Services;
using QuizGame.ViewModels;

namespace QuizGame.Views
{
    public partial class EditQuizView : UserControl
    {
        private readonly EditQuizViewModel _vm;

        public EditQuizView()
        {
            InitializeComponent();
            // Create ViewModel and connect it to the UI
            _vm = new EditQuizViewModel(new JsonQuizStorageService());
            DataContext = _vm;
        }

        // Add a new question to the selected quiz
        private void Add_Click(object sender, RoutedEventArgs e) => _vm.AddNewQuestion();

        // Delete the selected question
        private void Delete_Click(object sender, RoutedEventArgs e) => _vm.DeleteSelectedQuestion();

        // Apply changes from textboxes to the selected question
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the last edited TextBox updates its binding
            if (Keyboard.FocusedElement is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            // Validate inputs before applying
            if (!_vm.ApplyChangesToSelected())
                MessageBox.Show("Fill question text + 3 answers and a valid correct index (0..2).");
        }

        // Save all quizzes to file
        private async void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            await _vm.SaveAllAsync();
            MessageBox.Show("Saved.");
        }
    }
}
