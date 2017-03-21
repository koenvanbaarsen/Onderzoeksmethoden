using System;
using System.Collections.Generic;
using System.IO;

namespace Elo_Simulation
{
    class Program
    {
        const int numberOfSimulations = 10;
        static void Main(string[] args)
        {
            for (int i = 0; i < numberOfSimulations; i++)
            {
                var simulation = new Simulation(i.ToString());
                simulation.Simulate();
                var test = 0;
            }
            Console.ReadLine();
        }

    }
}
