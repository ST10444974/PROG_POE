using ChatBot_Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatBot_Final.ConsoleLogic
{
    public class ChatSession
    {
        private readonly ConversationContext context = new();
        private string userName;
        private bool nameRequested = false;

        public List<string> ProcessInput(string input)
        {
            var responses = new List<string>();

            // If name not yet set, treat first input as name
            if (string.IsNullOrEmpty(userName))
            {
                if (!nameRequested)
                {
                    nameRequested = true;
                    responses.Add("🤖 Please enter your name to begin:");
                    return responses;
                }

                userName = input.Trim();

                if (string.IsNullOrEmpty(userName))
                {
                    responses.Add("⚠️ Name can't be empty. Please enter your name:");
                }
                else
                {
                    responses.Add($"👋 Welcome, {userName}! I’m your cybersecurity assistant.");
                    responses.Add("💡 You can ask me things like 'phishing', 'passwords', or 'privacy'.");
                    responses.Add("Type 'exit' to end the conversation.");
                }

                return responses;
            }

            // Handle exit command
            if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                responses.Add("👋 Goodbye! Stay safe out there.");
                return responses;
            }

            // Handle empty input
            if (string.IsNullOrWhiteSpace(input))
            {
                responses.Add("⚠️ Please enter something so I can help you.");
                return responses;
            }

            // Get chatbot response
            try
            {
                string reply = CybersecurityResponder.GetResponse(input, context);
                responses.Add(reply);
            }
            catch
            {
                responses.Add("⚠️ Oops—something went wrong. Try rephrasing?");
            }

            return responses;
        }

        public bool HasName => !string.IsNullOrEmpty(userName);
        public string UserName => userName;
    }
}