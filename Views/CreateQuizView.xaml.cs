using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuizGame.Services;
using QuizGame.ViewModels;

namespace QuizGame.Views
{
    public partial class CreateQuizView : UserControl
    {
        private readonly CreateQuizViewModel _vm;

        public CreateQuizView()
        {
            InitializeComponent();

            // Skapar ViewModel och kopplar som DataContext
            _vm = new CreateQuizViewModel(new JsonQuizStorageService());
            DataContext = _vm;
        }

        // Tillåt endast siffror i fältet för CorrectIndex
        private void CorrectIndex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
                if (!char.IsDigit(c)) { e.Handled = true; break; }
        }

        // Säkerställ att sista TextBox uppdaterar sin binding innan vi läser värden
        private void ForceUpdateFocusedTextboxBinding()
        {
            if (Keyboard.FocusedElement is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        }

        // Lägg till fråga (kallas från XAML: Click="AddQuestion_Click")
        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            ForceUpdateFocusedTextboxBinding();
            var before = _vm.NewQuestions.Count;
            _vm.AddQuestion();

            // Enkel feedback om formuläret inte är komplett
            if (_vm.NewQuestions.Count == before)
                MessageBox.Show("Fill Question + 3 answers (and a valid Correct index 0..2).",
                                "Incomplete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Spara quiz (kallas från XAML: Click="SaveQuiz_Click")
        private async void SaveQuiz_Click(object sender, RoutedEventArgs e)
        {
            ForceUpdateFocusedTextboxBinding();
            try
            {
                await _vm.SaveQuizAsync();
                MessageBox.Show("Quiz saved.", "Success",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Felhantering vid sparande
                MessageBox.Show("Save failed:\n" + ex.Message, "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
