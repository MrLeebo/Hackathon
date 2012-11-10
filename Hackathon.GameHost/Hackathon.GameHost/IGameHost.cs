using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hackathon.GameHost.Domain;

namespace Hackathon.GameHost
{
    public interface IGameHost
    {
        event EventHandler<JSONEventArgs> OnPlayerJoined;
        event EventHandler<JSONEventArgs> OnPlayerGuessSubmitted;

        void StartRound(IEnumerable<Player> players, GameImage image, GameRound round);
        void UpdatePlayerStatus(Player player, GameRound round, string status);
        void StartJudgingRound(GameRound round, IDictionary<Player, string> guessByPlayer);
        void EndRound(GameRound round, IDictionary<Player, string> guessByPlayer, Player winner);        
    }
}
