using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace ChatBot_Final.Models
    {
    public class ContextTracker
    {
        public Dictionary<string, string> Memory { get; } = new();
        public bool AwaitingConfirmation { get; set; }
        public string? CurrentTopic { get; set; }
        public int FollowUpCount { get; set; } = 0;
    }
}
    
