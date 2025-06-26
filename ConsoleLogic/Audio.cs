using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ChatBot_Final.ConsoleLogic
{
    public static class Audio
    {
        private const string WELCOME_AUDIO_FILE = "./AI_VOICE_INTRO.wav";

        public static void PlayWelcomeAudio()
        {
            if (File.Exists(WELCOME_AUDIO_FILE))
            {
                try
                {
                    using (var player = new SoundPlayer(WELCOME_AUDIO_FILE))
                    {
                        player.Play();  // async play
                        Thread.Sleep(5000);  // optional: pause to allow time to hear intro
                    }
                }
                catch
                {
                    // Silent fail in WPF version
                }
            }
        }
    }
}
