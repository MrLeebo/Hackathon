using System.Web.Script.Serialization;

namespace Hackathon.GameHost.Domain
{
    public class Player
    {
        public Player(string name)
        {
            this.Name = name;
            this.Status = "Just joined";
            this.Score = 0;

        }

        public string Name { get; set; }
        public string Status { get; set; }
        public int Score { get; set; }

        public static Player FromJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            var player = serializer.Deserialize<PushPlayer>(json);

            return new Player(player.name);
        }
    }

    public class PushPlayer
    {
        public string name { get; set; }
    }

    public class PushPlayerGuess
    {
        public string name { get; set; }
        public string guess { get; set; }
    }
}
