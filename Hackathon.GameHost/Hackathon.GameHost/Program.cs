using System;
using Hackathon.GameHost.Lmgtfy;
using PusherClientDotNet;

namespace Hackathon.GameHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Random testing crap...");

            LmgtfyClient lmgtfyClient = new LmgtfyClient();
            LmgtfyResponse lmgtfyResponse = lmgtfyClient.Load();

            TumblrClient client = new TumblrClient();
            TumblrResponse result = client.Tagged("corn");

            Pusher.OnLog += Log;

            Console.WriteLine("Initializing the game...");

            var game = new Game();
            game.Start();

            Console.WriteLine("The game is running. Press Ctrl+C to kill it.");
            while (game.Running) { }

            Console.WriteLine("The game has stopped.");
        }

        private static void Log(object sender, PusherLogEventArgs e)
        {
            var text = e.Message;
            foreach (object o in e.Additional)
            {
                if (o == null)
                    continue;

                text += " | ";
                if (o is JsonData)
                {
                    text += Pusher.JSON.stringify(o);
                }
                else text += o.ToString();
            }

            Console.WriteLine(text);
        }
    }
}
