using System.Windows;
using QuizGame.Views;

namespace QuizGame
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Default view shown when the app starts
            MainContent.Content = new PlayQuizView();
        }

        // Switch to Play view
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PlayQuizView();
        }

        // Switch to Create view
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new CreateQuizView();
        }

        // Switch to Edit view
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new EditQuizView();
        }
    }
}
