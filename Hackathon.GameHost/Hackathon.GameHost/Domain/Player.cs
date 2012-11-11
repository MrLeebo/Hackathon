// ReSharper disable InconsistentNaming

using System.Web.Script.Serialization;

namespace Hackathon.GameHost.Domain
{
    public class Player
    {
        public string name { get; set; }
        public string type { get; set; }
        public int current_score { get; set; }
        public int rounds_played { get; set; }
        public string gravatar_url { get; set; }
        public string email { get; set; }
        public string guess { get; set; }
        public string private_channel { get; set; }

        public static Player FromJson(string json)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<Player>(json);
        }
    }
}
