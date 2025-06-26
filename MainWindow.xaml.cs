using ChatBot_Final.ConsoleLogic;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatBot_Final
{
    public partial class MainWindow : Window
    {
        private readonly ChatSession _session = new ChatSession();

        public MainWindow()
        {
            InitializeComponent();
            InitializeChat();
        }

        private void InitializeChat()
        {
            // Display initial bot message
            List<string> initialResponses = _session.ProcessInput("");
            foreach (string response in initialResponses)
            {
                AddMessage(response, false);
            }
        }

        private void AddMessage(string message, bool isUserMessage)
        {
            // Create message container
            StackPanel messageContainer = new StackPanel()
            {
                HorizontalAlignment = isUserMessage ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Margin = new Thickness(5)
            };

            // Create message bubble
            Border messageBubble = new Border()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                    isUserMessage ? "#007ACC" : "#333344")),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(12),
                MaxWidth = 600
            };

            // Create message text
            TextBlock messageText = new TextBlock()
            {
                Text = message,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 14
            };

            // Assemble components
            messageBubble.Child = messageText;
            messageContainer.Children.Add(messageBubble);
            ChatStack.Children.Add(messageContainer);

            // Auto-scroll to bottom
            ChatScrollViewer.ScrollToEnd();
        }

        private void SendMessage()
        {
            string userInput = txtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;

            // Add user message to UI
            AddMessage(userInput, true);
            txtUserInput.Clear();

            // Process through chat session
            List<string> responses = _session.ProcessInput(userInput);
            foreach (string response in responses)
            {
                AddMessage(response, false);
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e) => SendMessage();

        private void txtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift))
            {
                SendMessage();
                e.Handled = true;
            }
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