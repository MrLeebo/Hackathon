using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private readonly GameRound round;

        private GameState gameState = GameState.New;
        private DateTime roundFinishedInterval;

        public bool Running { get; set; }

        public Game()
        {
            this.gameHost = new GameHost();
            this.gameHost.PlayerJoined += OnPlayerJoined;
            this.gameHost.PlayerQuit += OnPlayerQuit;
            this.gameHost.GuessSubmitted += OnPlayerGuessSubmitted;
            this.gameHost.JudgeSubmitted += OnJudgeAnswerSubmitted;

            this.Running = true;
            this.gameHost.ShutDown += (o, e) => this.Running = false;

            this.imagePicker = new SimpleImagePicker();
            this.timer = new Timer(OnTimerInterval);
            this.round = new GameRound();
        }

        private void OnJudgeAnswerSubmitted(object sender, JudgingCompleteEventArgs e)
        {
            if (e.RoundWinner.round_id != this.round.Id)
                return;

            this.round.Winner = e.RoundWinner.winning_player;
        }

        private void OnPlayerGuessSubmitted(object sender, GuessSubmittedEventArgs e)
        {
            if (e.SubmittedGuess.round_id != this.round.Id)
                return;

            foreach (var activePlayer in activePlayers.Where(x => x.name == e.SubmittedGuess.player))
                activePlayer.guess = e.SubmittedGuess.guess;
        }

        private void OnPlayerQuit(object sender, ClientEventArgs e)
        {
            var player = e.ClientData.info;
            this.activePlayers.RemoveAll(x => x.name == player.name);
            this.pendingPlayers.RemoveAll(x => x.name == player.name);
        }

        private void OnPlayerJoined(object sender, ClientEventArgs e)
        {
            this.pendingPlayers.Add(e.ClientData.info);
        }

        public void Start()
        {
            this.gameHost.Initialize();
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
                        Console.WriteLine("Starting a new game...");

                        this.gameState = GameState.Initializing;
                        roundFinishedInterval = DateTime.Now;

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

                        foreach (var activePlayer in activePlayers.Where(x => string.IsNullOrEmpty(x.guess)))
                            activePlayer.guess = "No guess";

                        StartJudging();

                        gameState = GameState.Judging;
                        roundFinishedInterval = DateTime.Now.AddSeconds(JUDGING_TIME_IN_SECONDS);
                        return;
                    }
                case GameState.Judging:
                    {
                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        foreach (var activePlayer in activePlayers)
                        {
                            if (activePlayer.name == this.round.Winner)
                            {
                                activePlayer.current_score++;
                            }

                            activePlayer.rounds_played++;
                        }

                        gameState = GameState.GameOver;
                        roundFinishedInterval = DateTime.Now.AddSeconds(GAME_OVER_TIME_IN_SECONDS);

                        return;
                    }
                case GameState.GameOver:
                    {
                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        gameState = GameState.WaitingForGameStart;
                        roundFinishedInterval = DateTime.Now.AddSeconds(STARTING_TIME_IN_SECONDS);

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

            foreach (var activePlayer in activePlayers)
                activePlayer.guess = null;

            this.gameHost.RoundStarted(
                this.round.Id, 
                image.URL, 
                this.activePlayers.ToArray());
        }

        private void StartJudging()
        {
            this.gameHost.JudgingReady(this.round.Id, this.activePlayers.ToArray());
        }

        private string GetPlayerNames()
        {
            // Comma-delimit the player names.
            var names = activePlayers.Aggregate("", (accum, player) => accum + ", " + player.name);

            // Trim the comma from the start of the string.
            return names.Substring(2, names.Length - 2);
        }
    }
}
