using System;

namespace Hackathon.GameHost.Domain
{
    public class GuessSubmittedEventArgs : EventArgs
    {
        public SubmittedGuess SubmittedGuess { get; set; }

        public GuessSubmittedEventArgs(object data)
        {
            this.SubmittedGuess = data.ToObject<SubmittedGuess>();
        }
    }
}
