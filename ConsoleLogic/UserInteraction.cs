using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot_Final.ConsoleLogic
{
    public static class UserInteraction
    {
        public static string WelcomeUser(string name)
        {
            var sb = new StringBuilder();
            sb.AppendLine(AsciiArtDisplay.GetAsciiArt());
            sb.AppendLine($"\nWelcome, {name}!");
            sb.AppendLine(DrawBorder());
            sb.AppendLine("I'm your cybersecurity assistant here to help you:");
            sb.AppendLine("- Stay safe online");
            sb.AppendLine("- Recognize security threats");
            sb.AppendLine("- Protect your digital identity");
            sb.AppendLine(DrawBorder());
            return sb.ToString();
        }

        public static string DrawBorder() => "==================================================================";

        public static string Farewell()
        {
            var sb = new StringBuilder();
            sb.AppendLine(DrawBorder());
            sb.AppendLine("Stay safe online! Remember to:");
            sb.AppendLine("- Keep software updated");
            sb.AppendLine("- Be skeptical of unsolicited requests");
            sb.AppendLine("- Backup important data");
            sb.AppendLine(DrawBorder());
            return sb.ToString();
        }
    }
}