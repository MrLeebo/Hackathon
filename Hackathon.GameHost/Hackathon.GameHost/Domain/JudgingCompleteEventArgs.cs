using System;

namespace Hackathon.GameHost.Domain
{
    public class JudgingCompleteEventArgs : EventArgs
    {
        public RoundWinner RoundWinner { get; set; }

        public JudgingCompleteEventArgs(object data)
        {
            this.RoundWinner = data.ToObject<RoundWinner>();
        }
    }
}
