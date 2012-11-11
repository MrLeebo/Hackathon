using System;

namespace Hackathon.GameHost.Domain
{
    public class SubmittedGuess
    {
        public Guid round_id { get; set; }
        public string player { get; set; }
        public string guess { get; set; }
    }
}