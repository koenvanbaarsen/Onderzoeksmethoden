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
        const int noPlayers = 2000;
        const int beginRating = 1000;
        const string outputFolder = @"E:\Mijn documenten\Universiteit\Jaar 4\Periode C\Onderzoeksmethoden\Repository\Simulations\";

        const int NormalSteps = 150;
        const int NormalMaxValue = 2100;

        // Random number instantiation
        private readonly Random _rnd = new Random();

        public int id;
        int _k = 32;
        public Random _random;
        int _noRounds;

        StringBuilder playerEloRatingTable;

        public Simulation(int simulationID, Random random, int k, int noRounds)
        {
            id = simulationID;
            _random = random;
            _k = k;
            _noRounds = noRounds;
            playerEloRatingTable = new StringBuilder();
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
            //Array that will save the matches
            List<Match>[,] matchArray = new List<Match>[noPlayers, noPlayers];

            // Initialize players
            List<Player> players = new List<Player>();
            for (int i = 0; i < noPlayers; i++)
            {
                players.Add(new Player(i, RandomSkillFromNormal(), beginRating));

                //Initialize the lists where the matches will be saved
                for (int k = 0; k < noPlayers; k++)
                {
                    matchArray[i, k] = new List<Match>();
                }
            }

            //Add player elo ratings header
            playerEloRatingTable.Append(";");
            foreach (var player in players)
            {
                playerEloRatingTable.Append("Player " + player.id + ";");
            }
            playerEloRatingTable.Append(Environment.NewLine);

            // Play a certain amount of rounds
            for (int currentRound = 0; currentRound < _noRounds; currentRound++)
            {
                //Output the elo ratings
                playerEloRatingTable.Append("Round" + currentRound);
                OutputPlayerEloRatings(players);
                playerEloRatingTable.Append(Environment.NewLine);

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
                        matchArray[playerA.id, playerB.id].Add(match);
                    }
                }

                if (currentRound % 10 == 0)
                    Console.WriteLine("K:" + _k + " initialized round " + currentRound);
            }

            //Start with the header
            StringBuilder resultHeader = new StringBuilder();
            resultHeader.Append("PlayervsPlayer");
            for (int i = 0; i < _noRounds; i++)
            {
                resultHeader.Append(string.Format(";Round{0}", i));
            }
            resultHeader.Append(Environment.NewLine);

            StringBuilder resultCsvString = new StringBuilder();
            StringBuilder skillsCsvString = new StringBuilder();

            //And the body of the table
            foreach (Player playerA in players)
            {
                //Write the skill level
                skillsCsvString.Append(playerA.skill);
                skillsCsvString.Append(Environment.NewLine);
            }

                //    //Write away the score
                //    if (playerA.id % 10 == 0)
                //        Console.WriteLine(string.Format("K:" + _k + " Ordering player {0} for simulation {1}", playerA.id, id));
                //    for (int j = playerA.id + 1; j < noPlayers; j++)
                //    {
                //        Player playerB = players[j];
                //        string newEntry = string.Format("{0}vs{1}", playerA.id, playerB.id);

                //        var selectedMatches = matchArray[playerA.id, playerB.id].OrderBy(x => x.RoundNumber).ToList();
                //        foreach (var selectedMatch in selectedMatches)
                //        {
                //            newEntry += string.Format(";{0}", selectedMatch.EloScore);
                //        }

                //        resultCsvString.Append(newEntry);
                //        resultCsvString.Append(Environment.NewLine);
                //    }
                //}

                //Write away the strings
                StringBuilder resultsOutput = new StringBuilder();
            //WriteToFile(resultHeader.ToString() + resultCsvString.ToString(), "simulation_" + id.ToString());
            WriteToFile(skillsCsvString.ToString(), "skills_" + id.ToString());
            WriteToFile(playerEloRatingTable.ToString(), "eloratings_" + id.ToString());

            // Keep the console open, notify the user that something happened and append text to the file.
            Console.WriteLine(string.Format("K:" + _k + " The results of simulation {0} have been written to: {1}", id, outputFolder));

            return resultCsvString.ToString();
        }

        void OutputPlayerEloRatings(List<Player> players)
        {
            //playerEloRatingTable
            foreach (var player in players)
            { 
                playerEloRatingTable.Append(";");
                playerEloRatingTable.Append(player.elo);
            }

        }

        void WriteToFile(string text, string filename)
        {
            string folderForK = string.Format(@"{0}{1}\", outputFolder, _k);
            Directory.CreateDirectory(folderForK);
            string path = string.Format(@"{0}{1}.csv", folderForK, filename);
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

        void UpdateElo(Player winner, Player loser, double winProb)
        // Update the ELO Rating for loser and winner of the match.
        {
            double loseProb = 1 - winProb;
            winner.elo = (int)Math.Round(winner.elo + _k * (1 - winProb));
            loser.elo = (int)Math.Round(loser.elo + _k * (0 - loseProb));
        }
    }
}

