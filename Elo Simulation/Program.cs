﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elo_Simulation
{
    class Program
    {
        const int numberOfSimulations = 1;
        const int noRounds = 250;
        static int[] kValues = new int[] { 10, 16, 24, 32, 40 };
        const string outputFolder = @"E:\Mijn documenten\Universiteit\Jaar 4\Periode C\Onderzoeksmethoden\Repository\Simulations\";
        

        static void Main(string[] args)
        {
            CleanOutputFolder();

            Parallel.ForEach(kValues, (k) =>
            {
                DoSimulation(k);
            });

            Console.WriteLine("Finished all computations");
            Console.ReadLine();
        }

        static void DoSimulation(int k)
        {
            //Create a list for the simulations
            Random rnd = new Random();
            List<Simulation> simulations = new List<Simulation>();
            for (int i = 0; i < numberOfSimulations; i++)
            {
                simulations.Add(new Simulation(i, rnd, k, noRounds));
            }

            //Do them in parallel
            Dictionary<int, string> simulationResults = new Dictionary<int, string>();

            foreach(var currentSimulation in simulations)
            {
                var result = currentSimulation.Simulate();
                simulationResults[currentSimulation.id] = result;
            }

            //Start with the header
            StringBuilder allSimulationResults = new StringBuilder();
            //allSimulationResults.Append("PlayervsPlayer");
            //for (int i = 0; i < noRounds; i++)
            //{
            //    allSimulationResults.Append(string.Format(";Round{0}", i));
            //}
            //allSimulationResults.Append(Environment.NewLine);

            ////Finally combine them in the correct order
            //for (int i = 0; i < numberOfSimulations; i++)
            //{
            //    allSimulationResults.Append(simulationResults[i]);
            //}
            CombineSkills(k);
            CombineElos(k);

            //Console.WriteLine("Saved the combined files for K: " + k);
            WriteToFile(allSimulationResults.ToString(), k, "alle_results");
        }

        static void CleanOutputFolder()
        {
            DirectoryInfo di = new DirectoryInfo(outputFolder);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        static void CombineSkills(int k)
        {
            string combinedCsv = string.Empty;
            for (int i = 0; i < numberOfSimulations; i++)
            {
                combinedCsv += File.ReadAllText(string.Format(@"{0}{1}\skills_{2}.csv", outputFolder, k, i));
            }

            WriteToFile(combinedCsv, k, "alle_skills");
        }

        static void CombineElos(int k)
        {
            string combinedCsv = string.Empty;
            for (int i = 0; i < numberOfSimulations; i++)
            {
                combinedCsv += File.ReadAllText(string.Format(@"{0}{1}\eloratings_{2}.csv", outputFolder, k, i));
            }

            WriteToFile(combinedCsv, k, "alle_eloratings");
        }

        static void WriteToFile(string text, int k, string filename)
        {
            string path = string.Format(@"{0}{1}\{2}.csv", outputFolder, k, filename);
            using (StreamWriter sw = new StreamWriter(path, true))
                sw.Write(text);
        }
    }
}
