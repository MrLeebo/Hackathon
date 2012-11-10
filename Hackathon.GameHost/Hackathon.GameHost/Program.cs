using System;

namespace Hackathon.GameHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing the game...");

            using (var host = new GameHost())
            {
                host.OnPlayerGuessSubmitted += (o, e) => Console.WriteLine("Player '{0}''s guess is: {1}", e, e);
                host.OnPlayerJoined += (o, e) => Console.WriteLine("Player '{0}' joined.", e);
                host.OnPlayerQuit += (o, e) => Console.WriteLine("Player '{0}' quit.", e);

                bool shutdown = false;
                host.OnShutDown += (o, e) => shutdown = true;

                host.Initialize();

                Console.WriteLine("The game is running. Press Ctrl+C to kill it.");
                while (!shutdown) {}
            }

            Console.WriteLine("The game has stopped.");
        }
    }
}
