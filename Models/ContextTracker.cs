using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace ChatBot_Final.Models
    {
    public class ContextTracker
    {
       
        public string UserName { get; set; }
        public string CurrentTone { get; set; }
        public Dictionary<string, string> Memory { get; } = new Dictionary<string, string>();
        public bool AwaitingConfirmation { get; set; }
        public string CurrentTopic { get; set; }
        public int FollowUpCount { get; set; }
        public void ResetConversationState()
        {
            AwaitingConfirmation = false;
            CurrentTopic = null;
            FollowUpCount = 0;
            CurrentTone = null;
        }
    }
}

