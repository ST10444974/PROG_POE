using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChatBot_Final.ConsoleLogic
{
    public static class UserInteraction
    {
        // Stores the user's name after they introduce themselves 
        public static string? UserName;

        // Displays a personalized welcome message in the chat UI 
        public static void WelcomeUser(string name, StackPanel chatStack)
        {
            UserName = name;
            AddChatMessage(chatStack, $"Welcome, {name}! I'm your Cybersecurity Assistant.");
        }

        // Displays a farewell message when the user exits the chat 
        public static void Farewell(StackPanel chatStack)
        {
            AddChatMessage(chatStack, $"Goodbye, {UserName}! Stay safe online.");
        }

        // Adds a visual border (line) to separate sections in the chat 
        public static void DrawBorder(StackPanel chatStack)
        {
            AddChatMessage(chatStack, "==============================================");
        }

        // Adds a standard message to the chat UI using a TextBlock 
        public static void AddChatMessage(StackPanel chatStack, string message)
        {
            chatStack.Children.Add(new TextBlock
            {
                Text = message,                                        
                Foreground = System.Windows.Media.Brushes.White,       
                TextWrapping = System.Windows.TextWrapping.Wrap,       
                Margin = new System.Windows.Thickness(5)               
            });
        }
    }
}
