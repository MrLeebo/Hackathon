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
        void JudgingReady(Guid roundId, Player[] players);
        void JudgingComplete(Guid roundId, Player winner, Player[] player);        
    }
}
