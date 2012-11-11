using System;
using Hackathon.GameHost.Domain;

namespace Hackathon.GameHost
{
    public interface IGameHost
    {
        event EventHandler<ClientEventArgs> PlayerJoined;
        event EventHandler<ClientEventArgs> PlayerQuit;
        event EventHandler<GuessSubmittedEventArgs> GuessSubmitted;
        event EventHandler<JudgingCompleteEventArgs> JudgeSubmitted;

        event EventHandler ShutDown;

        void RoundStarted(Guid roundId, string imageUrl, Player[] players);
        void JudgingReady(Guid roundID, Player judge, Player[] players);
        void JudgingComplete(Guid roundId, Player winner, string actualSearch);
        void RoundComplete(Guid roundId, Player winner, Player[] players, string actualSearch);
        void ResetGame(Guid roundId);
    }
}
