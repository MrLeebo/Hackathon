// ReSharper disable InconsistentNaming
using System;

namespace Hackathon.GameHost.Domain
{
    public class RoundWinner
    {
        public Guid round_id { get; set; }
        public string winning_player { get; set; }
        public Player[] players { get; set; }
        public string actual_search { get; set; }
    }
}