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
        public static string? UserName;

        public static void WelcomeUser(string name, StackPanel chatStack)
        {
            UserName = name;
            AddChatMessage(chatStack, $"Welcome, {name}! I'm your Cybersecurity Assistant.");
        }

        public static void Farewell(StackPanel chatStack)
        {
            AddChatMessage(chatStack, $"Goodbye, {UserName}! Stay safe online.");
        }

        public static void DrawBorder(StackPanel chatStack)
        {
            AddChatMessage(chatStack, "==============================================");
        }

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