using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Elo_Simulation
{
    class Program
    {
        const int numberOfSimulations = 4;
        const string outputFolder = @"E:\Mijn documenten\Universiteit\Jaar 4\Periode C\Onderzoeksmethoden\Repository\Simulations\";

        static void Main(string[] args)
        {
            CleanOutputFolder();

            //Create a list for the simulations
            Random rnd = new Random();
            List<Simulation> simulations = new List<Simulation>();
            for (int i = 0; i < numberOfSimulations; i++)
            {
                simulations.Add(new Simulation(i, rnd));
            }

            //Do them in parallel
            Dictionary<int, string> simulationResults = new Dictionary<int, string>();
            Parallel.ForEach(simulations, (currentSimulation) =>
            {
                var result = currentSimulation.Simulate();
                simulationResults[currentSimulation.id] = result;
            });

            //Finally combine them in the correct order
            string allSimulationResults = string.Empty;
            for (int i = 0; i < numberOfSimulations; i++)
            {
                allSimulationResults += simulationResults[i];
            }
            CombineSkills();

            Console.WriteLine("Saved the combined files");
            WriteToFile(allSimulationResults, "alle_results");
            Console.ReadLine();
        }

        static void CleanOutputFolder()
        {
            DirectoryInfo di = new DirectoryInfo(outputFolder);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        static void CombineSkills()
        {
            string combinedCsv = string.Empty;
            for (int i = 0; i < numberOfSimulations; i++)
            {
                combinedCsv += File.ReadAllText(string.Format("{0}skills_{1}.csv", outputFolder, i));
            }

            WriteToFile(combinedCsv, "alle_skills");
        }

        static void WriteToFile(string text, string filename)
        {
            string path = string.Format(@"{0}{1}.csv", outputFolder, filename);
            using (StreamWriter sw = new StreamWriter(path, true))
                sw.Write(text);
        }
    }
}
