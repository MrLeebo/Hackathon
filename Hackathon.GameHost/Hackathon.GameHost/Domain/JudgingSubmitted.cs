using System;

namespace Hackathon.GameHost.Domain
{
    public class JudgingSubmitted
    {
        public Guid round_id { get; set; }
        public string winning_player { get; set; }
    }
}
