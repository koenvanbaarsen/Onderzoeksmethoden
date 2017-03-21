using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elo_Simulation
{
    public class Match
    {
        public Player PlayerA { get; set; }
        public Player PlayerB { get; set; }
        public int EloScore { get; set; }
        public int RoundNumber { get; set; }

        public Match(int roundNumber, Player playerA, Player playerB, int eloScore)
        {
            RoundNumber = roundNumber;
            PlayerA = playerA;
            PlayerB = playerB;
            EloScore = eloScore;
        }
    }
}
