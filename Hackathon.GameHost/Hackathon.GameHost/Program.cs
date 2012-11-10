using System;
using Hackathon.GameHost.Lmgtfy;

namespace Hackathon.GameHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing the game...");

            LmgtfyClient lmgtfyClient = new LmgtfyClient();
            LmgtfyResponse lmgtfyResponse = lmgtfyClient.Load();

            TumblrClient client = new TumblrClient();
            TumblrResponse result = client.Tagged("corn");
            Console.WriteLine("The game has stopped.");
        }
    }
}
