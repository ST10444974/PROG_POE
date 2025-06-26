using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using ChatBot_Final.ConsoleLogic; // Ensure your logic classes (AsciiArtDisplay, Audio, etc.) are in this namespace
using System.Windows.Documents;
using System.Windows.Media;
using ChatBot_Final.Models;
using System.Text.RegularExpressions;


namespace ChatBot_Final
{
    public partial class MainWindow : Window
    {
        private ContextTracker context = new();
        private string userName = "";
        private bool running = true;
        private bool nameCaptured = false;
        private TaskItem pendingTask = null;

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

            // Step 1: Capture user name
            if (!nameCaptured)
            {
                userName = input;
                nameCaptured = true;
                UserInteraction.WelcomeUser(userName, ChatStack);
                AddBotMessage($"{userName}, ask me about cybersecurity (or type 'exit')");
                return;
            }

            // Step 2: Handle exit
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("bye", StringComparison.OrdinalIgnoreCase))
            {
                running = false;
                UserInteraction.Farewell(ChatStack);
                return;
            }

            // Step 3: Add task via chatbot
            if (input.ToLower().StartsWith("add task -"))
            {
                string raw = input.Substring(10).Trim();

                if (!string.IsNullOrEmpty(raw))
                {
                    string fullDescription = $"{raw} to ensure your data is protected.";
                    pendingTask = new TaskItem
                    {
                        Title = "Cybersecurity Task",
                        Description = fullDescription,
                        IsCompleted = false
                    };

                    AddBotMessage($"Task added with the description \"{fullDescription}\". Would you like a reminder?");
                }
                else
                {
                    AddBotMessage("Please include a task description after 'Add task -'.");
                }
                return;
            }

            // Step 4: Handle reminder command
            if (pendingTask != null && Regex.IsMatch(input.ToLower(), @"remind me in \d+ (day|days|week|weeks)"))
            {
                var match = Regex.Match(input.ToLower(), @"remind me in (\d+)\s+(day|days|week|weeks)");

                if (match.Success)
                {
                    int amount = int.Parse(match.Groups[1].Value);
                    string unit = match.Groups[2].Value;
                    DateTime reminderDate = unit.StartsWith("week")
                        ? DateTime.Now.AddDays(amount * 7)
                        : DateTime.Now.AddDays(amount);

                    pendingTask.ReminderDate = reminderDate;

                    // Save the task to the global task list in TaskWindow
                    TaskWindow.PendingTasks.Add(pendingTask);

                    AddBotMessage($"Got it! I'll remind you in {amount} {unit}.");

                    pendingTask = null;
                }
                else
                {
                    AddBotMessage("Sorry, I couldn’t understand the reminder format. Try: 'Remind me in 3 days'.");
                }
                return;
            }

            // Step 5: Fallback to cybersecurity response
            UserInteraction.DrawBorder(ChatStack);

            string response;
            try
            {
                response = CybersecurityResponder.GetResponse(input, context);
            }
            catch
            {
                response = "Oops—something went wrong on my end. Can you try rephrasing?";
            }

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
