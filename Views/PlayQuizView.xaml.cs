// Views/PlayQuizView.xaml.cs
using System.Windows;
using System.Windows.Controls;
using QuizGame.Services;
using QuizGame.ViewModels;

namespace QuizGame.Views
{
    public partial class PlayQuizView : UserControl
    {
        public PlayQuizView()
        {
            InitializeComponent();
            // Connect the view to its ViewModel (data + logic)
            DataContext = new PlayQuizViewModel(new JsonQuizStorageService());
        }

        // Called when the user clicks an answer button
        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PlayQuizViewModel vm && sender is Button btn)
            {
                int index;
                // Get answer index from button tag and send it to the ViewModel
                if (int.TryParse(btn.Tag?.ToString(), out index))
                    vm.Answer(index);
            }
        }

        // Restart the quiz when "Play again" is clicked
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PlayQuizViewModel vm)
                vm.Restart();
        }
    }
}
