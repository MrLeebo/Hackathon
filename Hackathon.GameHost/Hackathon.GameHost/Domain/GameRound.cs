using System;

namespace Hackathon.GameHost.Domain
{
    public class GameRound
    {
        public Guid Id { get; set; }
        public Player Winner { get; set; }
        public string ActualSearch { get; set; }
    }
}
