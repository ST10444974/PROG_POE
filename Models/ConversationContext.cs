using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace ChatBot_Final.Models
    {
        public class ConversationContext
        {
            public bool AwaitingConfirmation { get; set; } = false;
            public string? CurrentTopic { get; set; } = null;
            public int FollowUpCount { get; set; } = 0;

            // Memory dictionary for storing user info like interest, level etc.
            public Dictionary<string, string> Memory { get; set; } = new();

            // Constructor (optional)
            public ConversationContext()
            {
                Memory = new Dictionary<string, string>();
            }
        }
    }
