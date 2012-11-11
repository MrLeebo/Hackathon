using System;
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

        public void RoundStarted(Guid roundId, string imageUrl, Player[] players)
        {
            var data =
                new RoundStart
                    {
                        round_id = roundId,
                        image_url = imageUrl,
                        players = players
                    };

            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "game-round-started",
                data);

            server.Trigger(request);
        }

        public void JudgingReady(Guid roundId, Player judge, Player[] players)
        {
            var data =
                new RoundStart
                    {
                        round_id = roundId,
                        players = players
                    };

            var request = new ObjectPusherRequest(
                judge.private_channel,
                "game-judging-ready",
                data);

            server.Trigger(request);
        }

        public void JudgingComplete(Guid roundId, Player winner, string actualSearch)
        {
            var data =
                new RoundWinner
                    {
                        round_id = roundId,
                        winning_player = (winner != null) ? winner.name : null,
                        actual_search = actualSearch
                    };

            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "game-judging-completed",
                data);

            server.Trigger(request);
        }

        public void RoundComplete(Guid roundId, Player winner, Player[] players, string actualSearch)
        {
            var data =
                new RoundWinner
                {
                    round_id = roundId,
                    winning_player = (winner != null) ? winner.name : null,
                    actual_search = actualSearch,
                    players = players
                };

            var request = new ObjectPusherRequest(
                PUBLIC_CHANNEL,
                "game-round-completed",
                data);

            server.Trigger(request);
        }

        public void ResetGame(Guid roundId)
        {
            var request = new ObjectPusherRequest(PUBLIC_CHANNEL, "game-round-reset", new {round_id = roundId});
            this.server.Trigger(request);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void OnMemberAdded(object data)
        {
            try
            {
                var clientData = data.ToObject<ClientData>();
                Channel privateChannel = client.Subscribe(clientData.info.private_channel);
                privateChannel.Bind(
                    "client-guess-submitted",
                    a =>
                    {
                        try
                        {
                            var triggerData = a.ToObject<SubmittedGuess>();
                            var request = new ObjectPusherRequest(PUBLIC_CHANNEL, "game-guess-submitted", triggerData);

                            server.Trigger(request);

                            if (GuessSubmitted != null)
                                GuessSubmitted(this, new GuessSubmittedEventArgs(a));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    });

                privateChannel.Bind(
                    "client-judging-submitted",
                    b =>
                    {
                        try
                        {
                            var triggerData = b.ToObject<JudgingSubmitted>();
                            var request = new ObjectPusherRequest(PUBLIC_CHANNEL, "game-judging-submitted", triggerData);

                            server.Trigger(request);

                            if (JudgeSubmitted != null)
                                JudgeSubmitted(this, new JudgingCompleteEventArgs(b));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    });

                if (PlayerJoined != null)
                    PlayerJoined(this, new ClientEventArgs(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void OnMemberRemoved(object data)
        {
            try
            {
                var clientData = data.ToObject<ClientData>();
                client.Unsubscribe(clientData.info.private_channel);

                if (PlayerQuit != null)
                    PlayerQuit(this, new ClientEventArgs(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
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
