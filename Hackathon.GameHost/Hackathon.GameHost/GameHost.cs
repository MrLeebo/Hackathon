using System;
using System.Collections.Generic;
using System.Linq;
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

        private const string PUBLIC_CHANNEL = "public";
        private const string SERVER_CHANNEL = "private-server";

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
            Pusher.channel_auth_endpoint = "http://beta.network360.com/pusher/auth/gamehome";
            var privateChannel = client.Subscribe(SERVER_CHANNEL);
            privateChannel.Bind("client-player-added", OnMemberAdded);
            privateChannel.Bind("client-player-dropped", OnMemberRemoved);

            privateChannel.Bind(
                "client-guess-submitted",
                d =>
                {
                    if (GuessSubmitted != null)
                        GuessSubmitted(this, new GuessSubmittedEventArgs(d));
                });

            privateChannel.Bind(
                "client-judging-completed",
                d =>
                    {
                        if (JudgeSubmitted != null)
                            JudgeSubmitted(this, new JudgingCompleteEventArgs(d));
                    });

            var publicChannel = client.Subscribe(PUBLIC_CHANNEL);
            publicChannel.Bind(
                "shutdown",
                d =>
                    {
                        if (ShutDown != null)
                            ShutDown(this, new JSONEventArgs(d));
                    });
        }

        public event EventHandler<ClientEventArgs> PlayerJoined;
        public event EventHandler<ClientEventArgs> PlayerQuit;
        public event EventHandler<GuessSubmittedEventArgs> GuessSubmitted;
        public event EventHandler<JudgingCompleteEventArgs> JudgeSubmitted;

        public event EventHandler ShutDown;

        public void RoundStarted(Guid round_id, string imageUrl, Player[] players)
        {
            var data =
                new RoundStart
                    {
                        round_id = round_id,
                        image_url = imageUrl,
                        players = players.ToArray()
                    };

            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "game-round-started",
                data);

            server.Trigger(request);
        }

        public void JudgingReady(Guid round_id, Player[] players)
        {
            var data =
                new RoundStart
                    {
                        round_id = round_id,
                        players = players
                    };

            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "game-judging-ready",
                data);

            server.Trigger(request);
        }

        public void JudgingComplete(Guid round_Id, Player winner, Player[] players)
        {
            var data =
                new RoundWinner
                    {
                        round_id = round_Id,
                        winning_player = winner.name,
                        players = players
                    };

            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "client-judging-completed",
                data);

            server.Trigger(request);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void OnMemberAdded(object data)
        {
            var clientData = data.ToObject<ClientData>();
            Channel privateChannel = client.Subscribe(clientData.info.private_channel);
            privateChannel.Bind(
                "game:player_guess_submitted",
                a =>
                {
                    if (GuessSubmitted != null)
                        GuessSubmitted(this, new GuessSubmittedEventArgs(a));
                });

            if (PlayerJoined != null)
                PlayerJoined(this, new ClientEventArgs(data));            
        }

        private void OnMemberRemoved(object data)
        {
            var clientData = data.ToObject<ClientData>();
            client.Unsubscribe(clientData.info.private_channel);

            if (PlayerQuit != null)
                PlayerQuit(this, new ClientEventArgs(data));
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                client.Unsubscribe(SERVER_CHANNEL);
                client.Unsubscribe(PUBLIC_CHANNEL);
                client.Disconnect();
            }

            disposed = true;
        }
    }
}
