using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using ChatBot_Final.ConsoleLogic; // Ensure your logic classes (AsciiArtDisplay, Audio, etc.) are in this namespace
using System.Windows.Documents;
using System.Windows.Media;
using ChatBot_Final.Models;

namespace ChatBot_Final
{
    public partial class MainWindow : Window
    {
        private ContextTracker context = new();
        private string userName = "";
        private bool running = true;
        private bool nameCaptured = false;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Simulate full console behavior on WPF start
            AsciiArtDisplay.Show(ChatStack);
            Audio.PlayWelcomeAudio();

            // Ask user for name in chat box
            AddBotMessage("Please Enter your name to begin.");
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = txtUserInput.Text.Trim();
            txtUserInput.Clear();

            if (string.IsNullOrEmpty(input)) return;

            AddUserMessage(input);

            // First input = Name
            if (!nameCaptured)
            {
                userName = input;
                nameCaptured = true;
                UserInteraction.WelcomeUser(userName, ChatStack);
                AddBotMessage($"{userName}, ask me about cybersecurity (or type 'exit')");
                return;
            }

            // Handle exit commands
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("bye", StringComparison.OrdinalIgnoreCase))
            {
                running = false;
                UserInteraction.Farewell(ChatStack);
                return;
            }

            // Handle empty input
            if (string.IsNullOrWhiteSpace(input))
            {
                AddBotMessage("Please enter something to continue.");
                return;
            }

            // Draw border before each new interaction
            UserInteraction.DrawBorder(ChatStack);

            string response;
            try
            {
                response = CybersecurityResponder.GetResponse(input, context);
            }
            catch (Exception)
            {
                response = "Oops—something went wrong on my end. Can you try rephrasing?";
            }

            // Typewriter text effect in chat
            await TextEffects.TypeWriter(response, ChatStack);
        }

        // === UI Helpers ===
        private void AddUserMessage(string message)
        {
            var text = new TextBlock
            {
                Text = $"You: {message}",
                Foreground = Brushes.LightGreen,
                Margin = new Thickness(5)
            };
            ChatStack.Children.Add(text);
        }

        private void AddBotMessage(string message)
        {
            var text = new TextBlock
            {
                Text = $"Bot: {message}",
                Foreground = Brushes.LightCyan,
                Margin = new Thickness(5)
            };
            ChatStack.Children.Add(text);
        }

        private void OpenTaskWindow_Click(object sender, RoutedEventArgs e)
        {
            TaskWindow taskWindow = new TaskWindow();
            taskWindow.Show();
        }

        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow();
            quizWindow.Show();
        }
    }
}
