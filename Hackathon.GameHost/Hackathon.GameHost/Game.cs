using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Hackathon.GameHost.Domain;

namespace Hackathon.GameHost
{
    public class Game
    {
        #region Fields

        private const int STARTING_TIME_IN_SECONDS = 5;
        private const int GUESSING_TIME_IN_SECONDS = 10;
        private const int JUDGING_TIME_IN_SECONDS = 10;
        private const int GAME_OVER_TIME_IN_SECONDS = 10;
        private const int INTERVAL_IN_SECONDS = 5;

        private readonly GameHost gameHost;
        private readonly IImagePicker imagePicker;
        private readonly Timer timer;

        private readonly List<Player> activePlayers = new List<Player>();
        private readonly List<Player> pendingPlayers = new List<Player>();

        private readonly GameRound round;
        private int judgeIndex = -1;

        private GameState gameState = GameState.New;
        private DateTime roundFinishedInterval;

        #endregion

        #region Public Properties

        public bool Running { get; set; }

        #endregion

        #region Constructor

        public Game()
        {
            this.gameHost = new GameHost();
            this.gameHost.PlayerJoined += OnPlayerJoined;
            this.gameHost.PlayerQuit += OnPlayerQuit;
            this.gameHost.GuessSubmitted += OnPlayerGuessSubmitted;
            this.gameHost.JudgeSubmitted += OnJudgeAnswerSubmitted;

            this.Running = true;
            this.gameHost.ShutDown += (o, e) => this.Running = false;

            this.imagePicker = new RandomImagePicker();
            var interval = TimeSpan.FromSeconds(INTERVAL_IN_SECONDS);
            this.timer = new Timer(interval.TotalMilliseconds);
            this.round = new GameRound();
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            this.gameHost.Initialize();
            this.timer.Elapsed += OnTimerInterval;
            this.timer.Start();
        }

        public void Stop()
        {
            this.gameHost.Dispose();
        }

        #endregion

        #region Event Handlers

        private void OnJudgeAnswerSubmitted(object sender, JudgingCompleteEventArgs e)
        {
            if (e.RoundWinner.round_id != this.round.Id)
                return;

            Console.WriteLine("The judge thinks " + e.RoundWinner.winning_player + " had the best answer!");

            var winner = this.activePlayers.FirstOrDefault(x => x.name == e.RoundWinner.winning_player);
            this.round.Winner = winner;
        }

        private void OnPlayerGuessSubmitted(object sender, GuessSubmittedEventArgs e)
        {
            if (e.SubmittedGuess.round_id != this.round.Id)
                return;

            Console.WriteLine(e.SubmittedGuess.player + " guesses '" + e.SubmittedGuess.guess + "'");

            foreach (var activePlayer in activePlayers.Where(x => x.name == e.SubmittedGuess.player))
                activePlayer.guess = e.SubmittedGuess.guess;
        }

        private void OnPlayerQuit(object sender, ClientEventArgs e)
        {
            var player = e.ClientData.info;

            Console.WriteLine(player.name + " has quit.");

            this.activePlayers.RemoveAll(x => x.name == player.name);
            this.pendingPlayers.RemoveAll(x => x.name == player.name);
        }

        private void OnPlayerJoined(object sender, ClientEventArgs e)
        {
            Console.WriteLine("New player joined: " + e.ClientData.info.name + " (" + e.ClientData.id + ")");

            var player = e.ClientData.info;
            player.type = "player";
            this.pendingPlayers.Add(e.ClientData.info);
        }

        private void OnTimerInterval(object sender, EventArgs e)
        {
            switch (gameState)
            {
                case GameState.New:
                    {
                        Console.WriteLine("Starting a new game... Waiting for players to show up.");

                        this.gameState = GameState.Initializing;
                        roundFinishedInterval = DateTime.Now;

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

                            Console.WriteLine("Current list of players: {0}", GetPlayerNames());
                        }

                        if (activePlayers.Count < 3)
                        {
                            Console.WriteLine("Not enough players...");
                            gameState = GameState.Initializing;
                            return;
                        }
                        
                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        Console.WriteLine("Starting a new game with these players: {0}", GetPlayerNames());

                        StartGame();

                        Console.WriteLine("The Waiting for Game to Start phase is over. Transitioning to Guessing.");

                        gameState = GameState.Guessing;
                        roundFinishedInterval = DateTime.Now.AddSeconds(GUESSING_TIME_IN_SECONDS);

                        return;
                    }
                case GameState.Guessing:
                    {
                        if (this.activePlayers.All(x => x.type != "judge"))
                        {
                            Console.WriteLine("The judge has quit the game.");
                            gameState = GameState.Initializing;
                            return;
                        }

                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        foreach (var activePlayer in activePlayers.Where(x => string.IsNullOrEmpty(x.guess)))
                            activePlayer.guess = "No guess";

                        StartJudging();

                        Console.WriteLine("The Guessing phase is over. Transitioning to Judging.");

                        gameState = GameState.Judging;
                        roundFinishedInterval = DateTime.Now.AddSeconds(JUDGING_TIME_IN_SECONDS);
                        return;
                    }
                case GameState.Judging:
                    {
                        if (this.activePlayers.All(x => x.type != "judge"))
                        {
                            Console.WriteLine("The judge has quit the game.");
                            gameState = GameState.Initializing;
                            return;
                        }

                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        bool winnerFound = false;
                        foreach (var activePlayer in activePlayers)
                        {
                            if (activePlayer == this.round.Winner)
                            {
                                winnerFound = true;
                                activePlayer.current_score++;
                            }

                            activePlayer.rounds_played++;
                        }

                        if (winnerFound)
                            Console.WriteLine(this.round.Winner + " is the winner!");
                        else
                            Console.WriteLine("No winner found...");

                        this.gameHost.JudgingComplete(this.round.Id, this.round.Winner, this.round.ActualTerm, this.activePlayers.ToArray());

                        Console.WriteLine("The Judging phase is over. Transitioning to Game Over in {0} second(s).", GAME_OVER_TIME_IN_SECONDS);

                        gameState = GameState.GameOver;
                        roundFinishedInterval = DateTime.Now.AddSeconds(GAME_OVER_TIME_IN_SECONDS);

                        return;
                    }
                case GameState.GameOver:
                    {
                        if (DateTime.Now < roundFinishedInterval)
                            return;

                        Console.WriteLine("The Game Over phase is over. Transitioning to Waiting for Game to Start.");

                        Console.WriteLine("Current Leaderboard:");
                        foreach (var activePlayer in activePlayers)
                            Console.WriteLine("Name: {0}, Score: {1}, Rounds Played: {2}, Type: {3}", activePlayer.name, activePlayer.current_score, activePlayer.rounds_played, activePlayer.type);
                        Console.WriteLine("There are {0} active players.", this.activePlayers.Count);

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

        #endregion

        #region Private Methods

        private void StartGame()
        {
            foreach (var activePlayer in activePlayers)
                activePlayer.type = "player";

            this.judgeIndex++;
            if (this.activePlayers.Count <= this.judgeIndex)
                this.judgeIndex = 0;

            var judge = this.activePlayers[judgeIndex];
            judge.type = "judge";
            Console.WriteLine("{0} is the judge!", judge.name);

            var pickedImage = imagePicker.Pick();
            Console.WriteLine("This image's url is {0}!", pickedImage.Url);
            var image = GameImage.FromImageData(pickedImage);

            this.round.Id = Guid.NewGuid();
            this.round.ActualTerm = pickedImage.Term;
            this.round.Winner = null;

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

        #endregion
    }
}
