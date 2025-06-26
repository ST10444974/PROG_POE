using System.Windows.Controls;
using System.Threading.Tasks;

namespace ChatBot_Final.ConsoleLogic
{
    public static class TextEffects
    {
        public static async Task TypeWriter(string text, StackPanel chatStack, int delay = 30)
        {
            TextBlock block = new()
            {
                Text = "",
                Foreground = System.Windows.Media.Brushes.Cyan,
                TextWrapping = System.Windows.TextWrapping.Wrap,
                Margin = new System.Windows.Thickness(5)
            };
            chatStack.Children.Add(block);

            foreach (char c in text)
            {
                block.Text += c;
                await Task.Delay(delay);
            }
        }
    }
}
