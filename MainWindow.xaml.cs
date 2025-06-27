using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using ChatBot_Final.ConsoleLogic;
using ChatBot_Final.Models;
using System.Windows.Media;
using System.Threading.Tasks;

namespace ChatBot_Final
{
    public partial class MainWindow : Window
    {
        private ContextTracker context = new();
        private string userName = "";
        private bool running = true;
        private bool nameCaptured = false;
        private TaskItem pendingTask = null;
        private bool awaitingReminderResponse = false;
        private bool awaitingReminderTime = false;
        private List<string> ActionLog = new(); 
        private const int MaxLogEntriesToShow = 10;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AsciiArtDisplay.Show(ChatStack);
            Audio.PlayWelcomeAudio();
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
                AddBotMessage($"{userName}, ask me about cybersecurity or give me a task (or type 'exit').");
                return;
            }

            // Step 2: Exit handling
            if (Regex.IsMatch(input, @"\b(exit|quit|bye)\b", RegexOptions.IgnoreCase))
            {
                running = false;
                UserInteraction.Farewell(ChatStack);
                return;
            }

            // Step 3: Handle Yes/No for reminder
            if (awaitingReminderResponse && pendingTask != null)
            {
                if (input.ToLower().Contains("yes"))
                {
                    AddBotMessage("Great! When should I remind you? (e.g., in 3 days, in 1 week)");
                    awaitingReminderResponse = false;
                    awaitingReminderTime = true;
                    return;
                }
                else if (input.ToLower().Contains("no"))
                {
                    AddBotMessage("Okay! No reminder set.");
                    TaskWindow.PendingTasks.Add(pendingTask);
                    LogAction($"Task added: '{pendingTask.Description}' (no reminder).");
                    pendingTask = null;
                    awaitingReminderResponse = false;
                    return;
                }
            }

            // Step 4: Set actual reminder time
            if (awaitingReminderTime && pendingTask != null)
            {
                var match = Regex.Match(input.ToLower(), @"in (\d+) (day|days|week|weeks)");
                if (match.Success)
                {
                    int amount = int.Parse(match.Groups[1].Value);
                    string unit = match.Groups[2].Value;
                    DateTime reminderDate = unit.StartsWith("week") ? DateTime.Now.AddDays(amount * 7) : DateTime.Now.AddDays(amount);

                    pendingTask.ReminderDate = reminderDate;
                    TaskWindow.PendingTasks.Add(pendingTask);
                    LogAction($"Reminder set for '{pendingTask.Description}' in {amount} {unit}.");
                    AddBotMessage($"Got it! I'll remind you in {amount} {unit}.");
                    pendingTask = null;
                    awaitingReminderTime = false;
                    return;
                }
                else
                {
                    AddBotMessage("Sorry, I didn’t catch that. Try something like 'in 3 days' or 'in 1 week'.");
                    return;
                }
            }

            // Step 5: Detect intent
            var intent = NLPIntentDetector.DetectIntent(input);

            if (intent != null)
            {
                if (intent.IntentType == "add_task")
                {
                    pendingTask = new TaskItem
                    {
                        Title = "Cybersecurity Task",
                        Description = intent.Description,
                        IsCompleted = false
                    };

                    if (intent.ReminderDays.HasValue)
                    {
                        pendingTask.ReminderDate = DateTime.Now.AddDays(intent.ReminderDays.Value);
                        TaskWindow.PendingTasks.Add(pendingTask);
                        LogAction($"Reminder set for '{pendingTask.Description}' in {intent.ReminderDays} day(s).");
                        AddBotMessage($"Reminder set for '{pendingTask.Description}' in {intent.ReminderDays} day(s).");
                        pendingTask = null;
                    }
                    else
                    {
                        AddBotMessage($"Task added: '{pendingTask.Description}'. Would you like to set a reminder?");
                        awaitingReminderResponse = true;
                    }
                    return;
                }

                if (intent.IntentType == "start_quiz")
                {
                    AddBotMessage("Launching cybersecurity quiz...");
                    OpenQuiz_Click(sender, e);
                    return;
                }

                if (intent.IntentType == "list_topics")
                {
                    AddBotMessage("I can help with:\n- Cybersecurity tips\n- Task creation\n- Reminders\n- Quiz\nAsk me!");
                    LogAction("Displayed help topics via NLP intent.");
                    return;
                }
            }

            // Step 6: View actions
            if (Regex.IsMatch(input.ToLower(), @"(activity log|what have you done|show log|show logs|log of actions)"))
            {
                if (ActionLog.Any())
                {
                    AddBotMessage("Here’s a summary of recent actions:");

                    var recentActions = ActionLog
                        .TakeLast(MaxLogEntriesToShow)
                        .Select((entry, index) => $"{index + 1}. {entry}");

                    foreach (var action in recentActions)
                        AddBotMessage(action);
                }
                else
                {
                    AddBotMessage("There’s nothing in the log yet. Try adding a task or starting the quiz.");
                }

                return;
            }

            // Step 7: Fallback to Responder
            UserInteraction.DrawBorder(ChatStack);

            string response;
            try
            {
                response = Responder.GetResponse(input, context);
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
            ChatStack.Children.Add(new TextBlock
            {
                Text = $"You: {message}",
                Foreground = Brushes.LightGreen,
                Margin = new Thickness(5)
            });
        }

        private void AddBotMessage(string message)
        {
            ChatStack.Children.Add(new TextBlock
            {
                Text = $"Bot: {message}",
                Foreground = Brushes.LightCyan,
                Margin = new Thickness(5)
            });
        }

        public static void LogAction(string action)
        {
            string timestamped = $"{DateTime.Now:HH:mm} - {action}";
            if (App.Current.MainWindow is MainWindow main)
            {
                if (main.ActionLog.Count >= 10)
                    main.ActionLog.RemoveAt(0);
                main.ActionLog.Add(timestamped);
            }
        }

        private void OpenTaskWindow_Click(object sender, RoutedEventArgs e)
        {
            new TaskWindow().Show();
        }

        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            new QuizWindow().Show();
        }
    }
}
