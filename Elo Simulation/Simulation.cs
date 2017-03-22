using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elo_Simulation
{
    class Simulation
    {
        // Constants
        const int K = 32;
        const int noRounds = 100;
        const int noPlayers = 32;
        const int beginRating = 1000;
        const string outputFolder = @"E:\Mijn documenten\Universiteit\Jaar 4\Periode C\Onderzoeksmethoden\Repository\Simulations\";

        // Random number instantiation
        private readonly Random _rnd = new Random();

        // Unique ID for a simulation
        private string _id;

        public Simulation(string simulationID)
        {
            _id = simulationID;
        }

        public void Simulate()
        {
            // Initialize players
            List<Player> players = new List<Player>();
            for (int i = 0; i < noPlayers; i++)
                players.Add(new Player(i, 1000 + 1000 * i / noPlayers, beginRating));

            //Keep a list of the rounds
            List<Match> matches = new List<Match>();

            // Play a certain amount of rounds
            for (int currentRound = 0; currentRound < noRounds; currentRound++)
            {
                foreach (Player playerA in players)
                {
                    // Make sure everyone plays against everyone
                    for (int j = playerA.id + 1; j < noPlayers; j++)
                    {
                        //Find another player
                        Player playerB = players[j];

                        //Do a match
                        Game(playerA, playerB);

                        //Create a variable for said match, and add it to the current round
                        Match match = new Match(currentRound, playerA, playerB, playerB.elo);
                        matches.Add(match);
                    }
                }
            }

            //Convert the rounds to a table

            //Start with the header
            string csvString = "PlayervsPlayer";
            for (int i = 0; i < noRounds; i++)
            {
                csvString += string.Format(";Round{0}", i);
            }
            csvString += Environment.NewLine;

            //And the body of the table
            matches.OrderBy(x => x.PlayerA.id).ThenBy(x => x.PlayerB.id).ThenBy(x => x.RoundNumber).ToList();
            foreach (Player playerA in players)
            {
                for (int j = playerA.id + 1; j < noPlayers; j++)
                {
                    Player playerB = players[j];
                    string newEntry = string.Format("{0}vs{1}", playerA.id, playerB.id);

                    var selectedMatches = matches.Where(x => x.PlayerA.id == playerA.id && x.PlayerB.id == playerB.id).OrderBy(x => x.RoundNumber).ToList();
                    foreach (var selectedMatch in selectedMatches)
                    {
                        newEntry += string.Format(";{0}", selectedMatch.EloScore);
                    }

                    csvString += newEntry;
                    csvString += Environment.NewLine;
                }
            }

            //Write away the string
            WriteToFile(csvString, _id);

            // Keep the console open, notify the user that something happened and append text to the file.
            Console.WriteLine(string.Format("The results of simulation {0} have been written to: {1}", _id, outputFolder));
        }

        void WriteToFile(string text, string filename)
        {
            string path = string.Format(@"{0}{1}.csv", outputFolder, filename);
            using (StreamWriter sw = new StreamWriter(path, true))
                sw.Write(text);
        }

        int Game(Player a, Player b)
        // Play a game with 2 players and calculate who wins based on skill.
        {
            double a_winProb = WinProb(a.skill, b.skill);

            if (_rnd.NextDouble() < a_winProb)
            {
                UpdateElo(a, b, a_winProb);
                return a.id;
            }
            else
            {
                UpdateElo(b, a, 1 - a_winProb);
                return b.id;
            }
        }

        static double WinProb(int a_skill, int b_skill)
        // Returns the probability that player a wins.
        {
            double Ra = Math.Pow(10, a_skill / 400);
            double Rb = Math.Pow(10, b_skill / 400);
            return Ra / (Ra + Rb);
        }

        static void UpdateElo(Player winner, Player loser, double winProb)
        // Update the ELO Rating for loser and winner of the match.
        {
            double loseProb = 1 - winProb;
            winner.elo = (int)Math.Round(winner.elo + K * (1 - winProb));
            loser.elo = (int)Math.Round(loser.elo + K * (0 - loseProb));
        }
    }
}

