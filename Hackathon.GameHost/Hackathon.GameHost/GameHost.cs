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
            server.Authenticate(GAME_CHANNEL, "socket-id");

            var presenceChannel = client.Subscribe(GAME_CHANNEL);
            presenceChannel.Bind("pusher:member_added", OnMemberAdded);
            presenceChannel.Bind("pusher:member_removed", OnMemberRemoved);

            presenceChannel.Bind(
                "game:player_guess_submitted",
                d =>
                    {
                        if (OnPlayerGuessSubmitted != null)
                            OnPlayerGuessSubmitted(this, new JSONEventArgs(d));
                    });

            var publicChannel = client.Subscribe(PUBLIC_CHANNEL);
            publicChannel.Bind(
                "game:shutdown",
                d =>
                    {
                        if (OnShutDown != null)
                            OnShutDown(this, EventArgs.Empty);
                    });
        }

        public event EventHandler<JSONEventArgs> OnPlayerJoined;
        public event EventHandler<JSONEventArgs> OnPlayerQuit;
        public event EventHandler<JSONEventArgs> OnPlayerGuessSubmitted;
        public event EventHandler OnShutDown;

        public void StartRound(IEnumerable<Player> players, GameImage image, GameRound round)
        {
        }

        public void UpdatePlayerStatus(Player player, GameRound round, string status)
        {
            throw new NotImplementedException();
        }

        public void StartJudgingRound(GameRound round, IDictionary<Player, string> guessByPlayer)
        {
            throw new NotImplementedException();
        }

        public void EndRound(GameRound round, IDictionary<Player, string> guessByPlayer, Player winner)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void OnMemberAdded(dynamic data)
        {
            Channel privateChannel = client.Subscribe(data.private_channel);
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
            client.Unsubscribe(data.private_channel);

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
