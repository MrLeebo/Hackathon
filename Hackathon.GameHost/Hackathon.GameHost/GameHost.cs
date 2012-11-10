using System;
using System.Collections.Generic;
using Hackathon.GameHost.Domain;
using PusherClientDotNet;
using PusherRESTDotNet;

namespace Hackathon.GameHost
{
    public class GameHost : IGameHost, IDisposable
    {
        private const string APP_ID = "31354";
        private const string APP_KEY = "6a4677394df2ac8f08d2";
        private const string SECRET = "0b981fba2c00f49d5dac";

        private const string PUBLIC_CHANNEL = "public-channel";
        private const string GAME_CHANNEL = "private-game-channel";

        private readonly Pusher client;
        private readonly IPusherProvider server;

        private bool disposed;

        public GameHost()
        {
            this.client = new Pusher(APP_KEY);
            this.server = new PusherProvider(APP_ID, APP_KEY, SECRET);
        }

        ~GameHost()
        {
            Dispose(false);
        }

        public void Initialize()
        {
            Pusher.channel_auth_endpoint = "https://beta.network360.com/pusher/auth/gamehome";
            var privateChannel = client.Subscribe(GAME_CHANNEL);
            privateChannel.Bind("client-player-added", OnMemberAdded);
            privateChannel.Bind("client-player-dropped", OnMemberRemoved);

            privateChannel.Bind(
                "game:player_guess_submitted",
                d =>
                {
                    if (OnPlayerGuessSubmitted != null)
                        OnPlayerGuessSubmitted(this, new JSONEventArgs(d));
                });

            var publicChannel = client.Subscribe(PUBLIC_CHANNEL);
            publicChannel.Bind(
                "gameshutdown",
                d =>
                    {
                        if (OnShutDown != null)
                            OnShutDown(this, new JSONEventArgs(d));
                    });
        }

        public event EventHandler<JSONEventArgs> OnPlayerJoined;
        public event EventHandler<JSONEventArgs> OnPlayerQuit;
        public event EventHandler<JSONEventArgs> OnPlayerGuessSubmitted;
        public event EventHandler<JSONEventArgs> OnShutDown;

        public void StartRound(IEnumerable<Player> players, GameImage image, GameRound round)
        {
            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "gamestartround",
                new
                    {
                        round = round,
                        players = players,
                        image = image
                    });

            server.Trigger(request);
        }

        public void UpdatePlayerStatus(Player player, GameRound round, string status)
        {
            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "gameupdateplayerstatus",
                new
                {
                    round = round,
                    player = player,
                    status = status
                });

            server.Trigger(request);
        }

        public void StartJudgingRound(GameRound round, IDictionary<Player, string> guessByPlayer)
        {
            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "gamestartjudginground",
                new
                {
                    round = round,
                    playerGuesses = guessByPlayer
                });

            server.Trigger(request);
        }

        public void EndRound(GameRound round, IDictionary<Player, string> guessByPlayer, Player winner)
        {
            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "gameendround",
                new
                {
                    round = round,
                    winner = winner,
                    playerGuesses = guessByPlayer
                });

            server.Trigger(request);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void OnMemberAdded(dynamic data)
        {
            Channel privateChannel = client.Subscribe(data.player.info.private_channel);
            privateChannel.Bind(
                "game:player_guess_submitted",
                a =>
                {
                    if (OnPlayerGuessSubmitted != null)
                        OnPlayerGuessSubmitted(this, new JSONEventArgs(a));
                });

            if (OnPlayerJoined != null)
                OnPlayerJoined(this, new JSONEventArgs(data));            
        }

        private void OnMemberRemoved(dynamic data)
        {
            client.Unsubscribe(data.player.info.private_channel);

            if (OnPlayerQuit != null)
                OnPlayerQuit(this, new JSONEventArgs(data));
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                client.Unsubscribe(GAME_CHANNEL);
                client.Unsubscribe(PUBLIC_CHANNEL);
                client.Disconnect();
            }

            disposed = true;
        }
    }
}
