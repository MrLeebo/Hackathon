using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using Hackathon.GameHost.Domain;

namespace Hackathon.GameHost
{
    public class Game
    {
        private const int STARTING_TIME_IN_SECONDS = 30;
        private const int GUESSING_TIME_IN_SECONDS = 15;
        private const int JUDGING_TIME_IN_SECONDS = 15;
        private const int GAME_OVER_TIME_IN_SECONDS = 10;
        private const int INTERVAL_IN_SECONDS = 1;

        private readonly GameHost gameHost;
        private readonly IImagePicker imagePicker;
        private readonly Timer timer;

        private readonly List<Player> activePlayers = new List<Player>();
        private readonly List<Player> pendingPlayers = new List<Player>();

        private readonly Dictionary<Player, string> playerGuesses = new Dictionary<Player, string>(); 

        private readonly GameRound round;

        private GameState gameState = GameState.New;
        private DateTime roundFinishedInterval;

        public Game()
        {
            this.gameHost = new GameHost();
            this.gameHost.OnPlayerJoined += OnPlayerJoined;
            this.gameHost.OnPlayerQuit += OnPlayerQuit;
            this.gameHost.OnPlayerGuessSubmitted += OnPlayerGuessSubmitted;

            this.imagePicker = new SimpleImagePicker();
            this.timer = new Timer(OnTimerInterval);
            this.round = new GameRound();
        }

        private void OnPlayerGuessSubmitted(object sender, JSONEventArgs e)
        {
            var serializer = new JavaScriptSerializer();
            var playerGuess = serializer.Deserialize<PushPlayerGuess>(e.JSONData);
            var player = new Player(playerGuess.name);

            this.playerGuesses.Add(player, playerGuess.guess);
        }

        private void OnPlayerQuit(object sender, JSONEventArgs e)
        {
            var player = new Player(e.JSONData);
            this.activePlayers.RemoveAll(x => x.Name == player.Name);
        }

        private void OnPlayerJoined(object sender, JSONEventArgs e)
        {
            var player = new Player(e.JSONData);
            this.pendingPlayers.Add(player);
        }

        public void Start()
        {
            this.gameHost.Initialize();
            this.gameState = GameState.Initializing;

            this.timer.Change(INTERVAL_IN_SECONDS, INTERVAL_IN_SECONDS);
        }

        public void Stop()
        {
            this.gameHost.Dispose();
        }

        private void OnTimerInterval(object state)
        {
            switch (gameState)
            {
                case GameState.New:
                    {
                        this.Start();
                        return;
                    }
                case GameState.Initializing:
                    {
                        gameState = GameState.WaitingForGameStart;
                        roundFinishedInterval = DateTime.Now.AddSeconds(STARTING_TIME_IN_SECONDS);
                        return;
                    }
                case GameState.WaitingForGameStart:
                    {
                        if (pendingPlayers.Count > 0)
                        {
                            Console.WriteLine("Adding new pending players to active list.");

                            activePlayers.AddRange(pendingPlayers);
                            pendingPlayers.Clear();

                            Console.WriteLine("Current list of players: " + GetPlayerNames());
                        }

                        if (activePlayers.Count < 3)
                        {
                            Console.WriteLine("Too few players to start a game. Resetting timer.");
                            gameState = GameState.Initializing;
                            return;
                        }
                        
                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        Console.WriteLine("Starting a new game with these players: " + GetPlayerNames());

                        StartGame();

                        gameState = GameState.Guessing;
                        roundFinishedInterval = DateTime.Now.AddSeconds(GUESSING_TIME_IN_SECONDS);

                        return;
                    }
                case GameState.Guessing:
                    {
                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        roundFinishedInterval = DateTime.Now.AddSeconds(JUDGING_TIME_IN_SECONDS);
                        return;
                    }
                case GameState.Judging:
                    {
                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        roundFinishedInterval = DateTime.Now.AddSeconds(GAME_OVER_TIME_IN_SECONDS);

                        return;
                    }
                case GameState.GameOver:
                    {
                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        gameState = GameState.WaitingForGameStart;

                        return;
                    }
                default:
                    {
                        Console.WriteLine("Game is not in a recognized state.");
                        gameState = GameState.New;
                        return;
                    }
            }
        }

        private void StartGame()
        {
            var pickedImage = imagePicker.Pick();
            var image = GameImage.FromImageData(pickedImage);

            this.round.Id = Guid.NewGuid();

            this.gameHost.StartRound(this.activePlayers, image, this.round);
        }

        private string GetPlayerNames()
        {
            // Comma-delimit the player names.
            var names = activePlayers.Aggregate("", (accum, player) => accum + ", " + player.Name);

            // Trim the comma from the start of the string.
            return names.Substring(2, names.Length - 2);
        }
    }
}
