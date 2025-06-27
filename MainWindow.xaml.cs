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

/*
References used for this Assignment Final POE: 

Using Regular Expressions for Command Parsing in C#
Microsoft. (2023). Regular expressions in .NET. [online] Available at: https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions [Accessed 27 Jun. 2025].

Asynchronous Programming with async and await in WPF
Microsoft. (2023). Asynchronous programming with async and await. [online] Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/ [Accessed 27 Jun. 2025].

Managing Lists with Skip() and Take() for Paginated Display
Microsoft. (2023). Standard query operators overview (LINQ to Objects). [online] Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/standard-query-operators-overview [Accessed 27 Jun. 2025].

Detecting NLP-like Intents with Simple Keyword Matching in C#
Hossain, K. (2020). How to implement simple NLP-based chatbot using keyword matching in C#. [online] Available at: https://www.codeproject.com/Articles/5283090/Creating-a-Simple-Chatbot-in-Csharp [Accessed 27 Jun. 2025].

WPF UI Manipulation: Dynamically Adding TextBlocks to StackPanel
Microsoft. (2023). TextBlock Class (System.Windows.Controls). [online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.textblock [Accessed 27 Jun. 2025].

Creating a Task Scheduler with Reminders in .NET
Microsoft. (2023). Timers in .NET. [online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.threading.timer [Accessed 27 Jun. 2025].
*/

namespace ChatBot_Final
{
    public partial class MainWindow : Window
    {
        // Fields for conversation state tracking 
        private ContextTracker context = new();                     // Tracks user's current topic, tone, memory
        private string userName = "";                               // Stores user's name after it's entered
        private bool nameCaptured = false;                          // Flag to check if the user's name is known
        private TaskItem pendingTask = null;                        // Temporarily stores a task being created
        private bool awaitingReminderResponse = false;              // Waiting for user to confirm reminder
        private bool awaitingReminderTime = false;                  // Waiting for user to give reminder time
        private List<string> ActivityLog = new();                   // Stores all chatbot activity logs
        private int DisplayedLogIndex = 0;                          // Keeps track of logs shown
        private const int LogPageSize = 5;                          // Number of log entries to show at a time

        // Constructor: Initializes WPF components and triggers greeting 
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        // Displays welcome ASCII art and audio, then prompts for name 
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AsciiArtDisplay.Show(ChatStack);
            Audio.PlayWelcomeAudio();
            AddBotMessage("Please Enter your name to begin.");
        }

        // Handles user input when the Send button is clicked 
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = txtUserInput.Text.Trim();
            txtUserInput.Clear();
            if (string.IsNullOrEmpty(input)) return;

            AddUserMessage(input); // Show user's input in the chat

            //  Step 1: Capture user's name 
            if (!nameCaptured)
            {
                userName = input;
                nameCaptured = true;
                UserInteraction.WelcomeUser(userName, ChatStack);
                AddBotMessage($"{userName}, ask me about cybersecurity or give me a task (or type 'exit').");
                return;
            }

            //  Step 2: Handle "exit" commands 
            if (Regex.IsMatch(input, @"\b(exit|quit|bye)\b", RegexOptions.IgnoreCase))
            {
                UserInteraction.Farewell(ChatStack);
                return;
            }

            //  Step 3: Handle yes/no response to reminder confirmation 
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

            //  Step 4: Handle natural language time input for reminders 
            if (awaitingReminderTime && pendingTask != null)
            {
                var match = Regex.Match(input.ToLower(), @"in (\d+) (day|days|week|weeks)");
                if (match.Success)
                {
                    int amount = int.Parse(match.Groups[1].Value);
                    string unit = match.Groups[2].Value;
                    DateTime reminderDate = unit.StartsWith("week")
                        ? DateTime.Now.AddDays(amount * 7)
                        : DateTime.Now.AddDays(amount);

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
                    AddBotMessage("Sorry, try something like 'in 3 days' or 'in 1 week'.");
                    return;
                }
            }

            // Step 5: Manual task detection using keyword pattern
            var manualTaskMatch = Regex.Match(input, @"add\s+task\s*[:-]?\s*(.*)", RegexOptions.IgnoreCase);
            if (manualTaskMatch.Success)
            {
                string desc = manualTaskMatch.Groups[1].Value.Trim();
                if (!string.IsNullOrEmpty(desc))
                {
                    pendingTask = new TaskItem
                    {
                        Title = "Cybersecurity Task",
                        Description = desc,
                        IsCompleted = false
                    };
                    AddBotMessage($"Task added: '{desc}'. Would you like to set a reminder?");
                    awaitingReminderResponse = true;
                    return;
                }
            }

            // Step 6: Use NLP to detect intent like task or quiz 
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
                        LogAction($"Reminder set for '{pendingTask.Description}' in {intent.ReminderDays} days.");
                        AddBotMessage($"Reminder set for '{pendingTask.Description}' in {intent.ReminderDays} days.");
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
                    LogAction("Quiz started via NLP intent.");
                    AddBotMessage("Launching quiz...");
                    OpenQuiz_Click(sender, e);
                    return;
                }

                if (intent.IntentType == "list_topics")
                {
                    LogAction("Displayed help topics via NLP intent.");
                    AddBotMessage("I can help with:\n- Cybersecurity tips\n- Task creation\n- Reminders\n- Quiz\nAsk me!");
                    return;
                }
            }

            // Step 7: Show recent activity logs
            if (Regex.IsMatch(input.ToLower(), @"(activity log|what have you done|show logs|show activity|show more)"))
            {
                if (!ActivityLog.Any())
                {
                    AddBotMessage("There’s nothing in the log yet.");
                    return;
                }

                int entriesLeft = ActivityLog.Count - DisplayedLogIndex;
                if (entriesLeft <= 0)
                {
                    AddBotMessage("You’ve seen the full log already.");
                    return;
                }

                var logSlice = ActivityLog.Skip(DisplayedLogIndex).Take(LogPageSize).ToList();
                AddBotMessage("Here’s your recent activity:");
                for (int i = 0; i < logSlice.Count; i++)
                    AddBotMessage($"{DisplayedLogIndex + i + 1}. {logSlice[i]}");

                DisplayedLogIndex += logSlice.Count;
                if (DisplayedLogIndex < ActivityLog.Count)
                    AddBotMessage("Type 'Show more' to see the next actions.");
                return;
            }

            // Step 8: Default chatbot response 
            UserInteraction.DrawBorder(ChatStack); // Decorative border

            string response;
            try
            {
                response = Responder.GetResponse(input, context);

                // Log topic if available
                if (!string.IsNullOrEmpty(context.CurrentTopic))
                {
                    LogAction($"Cybersecurity response given for topic: '{context.CurrentTopic}'");
                }
                else
                {
                    LogAction("Bot response was given.");
                }
            }
            catch
            {
                response = "Oops—something went wrong on my end. Can you try rephrasing?";
            }

            await TextEffects.TypeWriter(response, ChatStack); // Animated bot response
        }

        // Adds a user message to the chat UI 
        private void AddUserMessage(string message)
        {
            ChatStack.Children.Add(new TextBlock
            {
                Text = $"You: {message}",
                Foreground = Brushes.LightGreen,
                Margin = new Thickness(5)
            });
        }

        // Adds a bot message to the chat UI 
        private void AddBotMessage(string message)
        {
            ChatStack.Children.Add(new TextBlock
            {
                Text = $"Bot: {message}",
                Foreground = Brushes.LightCyan,
                Margin = new Thickness(5)
            });
        }

        // Logs a chatbot action to the activity log with time 
        public static void LogAction(string action)
        {
            if (App.Current.MainWindow is MainWindow main)
            {
                string timestamped = $"{DateTime.Now:HH:mm} - {action}";
                main.ActivityLog.Add(timestamped);
            }
        }

        // Opens the task window manually 
        private void OpenTaskWindow_Click(object sender, RoutedEventArgs e)
        {
            new TaskWindow().Show();
        }

        // Opens the quiz window manually 
        private void OpenQuiz_Click(object sender, RoutedEventArgs e)
        {
            new QuizWindow().Show();
        }
    }
}
