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
            if (lower.Contains("remind me to") || lower.Contains("add a task") || lower.Contains("set a reminder to"))
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

            // Advanced quiz detection with keyword co-occurrence
            var tokens = lower.Split(new char[] { ' ', '.', ',', '!', '?' },
                                    StringSplitOptions.RemoveEmptyEntries);

            // Define keyword categories for quiz detection
            var actionVerbs = new HashSet<string> { "start", "begin", "launch", "take", "commence" };
            var quizNouns = new HashSet<string> { "quiz", "test", "exam", "assessment", "questions" };

            // Check for co-occurrence of action verbs and quiz nouns
            bool hasAction = tokens.Any(t => actionVerbs.Contains(t));
            bool hasQuiz = tokens.Any(t => quizNouns.Contains(t));

            if (hasAction && hasQuiz)
            {
                return new NLPIntent { IntentType = "start_quiz" };
            }

            // Enhanced topic listing detection with token-based matching
            var topicTriggers = new HashSet<string> { "help", "topics", "options", "what", "list" };
            bool hasTopicTrigger = tokens.Any(t => topicTriggers.Contains(t)) ||
                                  lower.Contains("what can you do") ||
                                  lower.Contains("show commands");

            if (hasTopicTrigger)
            {
                return new NLPIntent { IntentType = "list_topics" };
            }

            return null;
        }
    }
}