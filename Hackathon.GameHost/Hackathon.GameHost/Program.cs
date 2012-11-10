using System;
using PusherClientDotNet;

namespace Hackathon.GameHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing the game...");

            Pusher.OnLog += Log;

            using (var host = new GameHost())
            {
                host.OnPlayerGuessSubmitted += (o, e) => Console.WriteLine("Player '{0}''s guess is: {1}", e, e);
                host.OnPlayerJoined += (o, e) => Console.WriteLine("Player '{0}' joined.", e);
                host.OnPlayerQuit += (o, e) => Console.WriteLine("Player '{0}' quit.", e);

                bool shutdown = false;
                host.OnShutDown += (o, e) => Console.WriteLine(Pusher.JSON.stringify(e.JSONData)); //shutdown = true;

                host.Initialize();

                Console.WriteLine("The game is running. Press Ctrl+C to kill it.");
                while (!shutdown) {}
            }

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
