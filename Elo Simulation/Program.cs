using System;
using System.Collections.Generic;
using System.IO;

namespace EloRating
{
    class Program
    {
        // Constants
        static int K = 32;
        static int noRounds = 100;
        static int noPlayers = 6;
        static int beginRating = 1000;

        // Random number instantiation
        private static readonly Random _rnd = new Random();

        static void Main(string[] args)
        {
            // Initialize players
            List<Player> players = new List<Player>();
            for (int i = 0; i < noPlayers; i++)
                players.Add(new Player(i, 1000 + 1000 * i / noPlayers, beginRating));

            //Write the variable names to the CSV
            WriteToFile("PlayerA_ID;PlayerA_EloScore;PlayerB_ID;PlayerB_EloScore" + Environment.NewLine);

            // Play a certain amount of rounds
            for (int i = 0; i < noRounds; i++)
                foreach (Player p in players)
                    // Make sure everyone plays against everyone
                    for (int j = p.id + 1; j < noPlayers; j++)
                    {
                        Game(p, players[j]);
                        var result = string.Format("P{0};{1};P{2};{3}{4}", p.id, p.elo, players[j].id, players[j].elo, Environment.NewLine);
                        WriteToFile(result);
                    }

            // Keep the console open, notify the user that something happened and append text to the file.
            Console.WriteLine("Results have been written to: \"D:\\MyProjects\\EloRating\\EloRating\\_data\" ");
            Console.ReadLine();

        }

        static int Game(Player a, Player b)
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

        static void WriteToFile(string text)
        {
            using (StreamWriter sw = new StreamWriter(@"E:\Mijn documenten\Universiteit\Jaar 4\Periode C\Onderzoeksmethoden\Elo Simulation\Test.csv", true))
                sw.Write(text);
        }
    }

    class Player
    {
        public int id;
        public int skill;
        public int elo;

        public Player(int _id, int _skill, int _elo)
        {
            id = _id;
            skill = _skill;
            elo = _elo;
        }
    }

}
