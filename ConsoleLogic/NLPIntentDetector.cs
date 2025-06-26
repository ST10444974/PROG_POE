using System;
using System.Text.RegularExpressions;

namespace ChatBot_Final.ConsoleLogic
{
    public class NLPIntent
    {
        public string IntentType { get; set; }
        public string Description { get; set; }
        public int? ReminderDays { get; set; }
    }

    public static class NLPIntentDetector
    {
        public static NLPIntent? DetectIntent(string input)
        {
            string lower = input.ToLower();

            // Match task commands like "add a task to ..." or "remind me to ..."
            if (lower.Contains("remind me to") || lower.Contains("add a task to") || lower.Contains("set a reminder to"))
            {
                // Extract description
                var match = Regex.Match(input, @"(remind me to|add a task to|set a reminder to)\s+(.+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string description = match.Groups[2].Value.Trim();

                    // Check if "tomorrow" is mentioned
                    if (lower.Contains("tomorrow"))
                    {
                        return new NLPIntent
                        {
                            IntentType = "add_task",
                            Description = description,
                            ReminderDays = 1
                        };
                    }

                    return new NLPIntent
                    {
                        IntentType = "add_task",
                        Description = description,
                        ReminderDays = null // Ask user when
                    };
                }
            }

            // Start quiz
            if (lower.Contains("start quiz") || lower.Contains("launch quiz"))
            {
                return new NLPIntent { IntentType = "start_quiz" };
            }

            // Ask what bot can do
            if (lower.Contains("what can you do") || lower.Contains("help") || lower.Contains("topics"))
            {
                return new NLPIntent { IntentType = "list_topics" };
            }

            return null;
        }
    }
}
