using System;

namespace Hackathon.GameHost.Domain
{
    public class RoundStart
    {
        public Guid round_id { get; set; }
        public string image_url { get; set; }
        public Player[] players { get; set; }
    }
}