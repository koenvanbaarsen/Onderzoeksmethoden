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
        const int noPlayers = 200;
        const int beginRating = 1000;
        const string outputFolder = @"E:\Mijn documenten\Universiteit\Jaar 4\Periode C\Onderzoeksmethoden\Repository\Simulations\";

        const int NormalSteps = 150;
        const int NormalMaxValue = 2100;

        // Random number instantiation
        private readonly Random _rnd = new Random();

        // Unique ID for a simulation
        public int id;
        public Random _random;

        public Simulation(int simulationID, Random random)
        {
            id = simulationID;
            _random = random;
        }

        private int RandomSkillFromNormal()
        {
            int count = 0;
            int val = 0;

            while (++count * NormalSteps <= NormalMaxValue) val += _random.Next(NormalSteps);

            return val;
        }


        public string Simulate()
        {
            // Initialize players
            List<Player> players = new List<Player>();
            for (int i = 0; i < noPlayers; i++)
            {
                players.Add(new Player(i, RandomSkillFromNormal(), beginRating));
            }

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

            //Start with the header
            string resultCsvString = "PlayervsPlayer";
            for (int i = 0; i < noRounds; i++)
            {
                resultCsvString += string.Format(";Round{0}", i);
            }
            resultCsvString += Environment.NewLine;

            string skillsCsvString = string.Empty;

            //And the body of the table
            matches.OrderBy(x => x.PlayerA.id).ThenBy(x => x.PlayerB.id).ThenBy(x => x.RoundNumber).ToList();
            foreach (Player playerA in players)
            {
                //Write the skill level
                skillsCsvString += playerA.skill;
                skillsCsvString += Environment.NewLine;

                //Write away the score
                Console.WriteLine(string.Format("Ordering player {0} for simulation {1}", playerA.id, id));
                for (int j = playerA.id + 1; j < noPlayers; j++)
                {
                    Player playerB = players[j];
                    string newEntry = string.Format("{0}vs{1}", playerA.id, playerB.id);

                    var selectedMatches = matches.Where(x => x.PlayerA.id == playerA.id && x.PlayerB.id == playerB.id).OrderBy(x => x.RoundNumber).ToList();
                    foreach (var selectedMatch in selectedMatches)
                    {
                        newEntry += string.Format(";{0}", selectedMatch.EloScore);
                    }

                    resultCsvString += newEntry;
                    resultCsvString += Environment.NewLine;
                }
            }

            //Write away the strings
            WriteToFile(resultCsvString, "simulation_" + id.ToString());
            WriteToFile(skillsCsvString, "skills_" + id.ToString());

            // Keep the console open, notify the user that something happened and append text to the file.
            Console.WriteLine(string.Format("The results of simulation {0} have been written to: {1}", id, outputFolder));

            return resultCsvString;
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

