using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LichessTournamentAggregator.App
{
    public static class Program
    {
        private const string RepoUrl = "https://github.com/eduherminio/LichessTournamentAggregator";
        private static readonly string FailureMessage = "The program has failed unexpectedly," +
            $" please raise an issue in {RepoUrl}/issues (or just contact me)," +
            $" including the following info:{Environment.NewLine}";

        public static async Task Main(string[] args)
        {
            var tournaments = args.Length > 0
                ? args.ToList()
                : AskForTournaments().ToList();

            var aggregatedArgs = $"*\t{string.Join($"{Environment.NewLine}*\t", tournaments)}\n";
            try
            {
                Console.WriteLine($"Aggregating tournaments:{Environment.NewLine}{aggregatedArgs}");

                await AggregateResultsAndCreateCsvFile(tournaments);
            }
            catch (ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Please make this is the tournament url you want to aggregate:");
                Console.WriteLine($"*\t{e.ParamName}");
            }
            catch (HttpRequestException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("There may be some issues with Lichess server or you've reached the API limit.");
                Console.WriteLine($"Please try again in a few minutes. If the problem persists, raise an issue in {RepoUrl}/issues");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(FailureMessage);
                Console.ResetColor();
                Console.WriteLine("Args:\n" + aggregatedArgs
                    + "\nException: " + e.Message + Environment.NewLine + e.StackTrace);
            }
            finally
            {
                Console.ResetColor();
                Console.WriteLine("\nPress any key to close this window");
                Console.ReadKey();
            }
        }

        private static IEnumerable<string> AskForTournaments()
        {
            Console.WriteLine("\nType or paste the Lichess tournament ids or full tournament urls that you want to aggregate, separated by a new line.");
            Console.WriteLine("Hit enter again when finished.");

            while (true)
            {
                string arg = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(arg))
                {
                    break;
                }

                yield return arg;
            }
        }

        private static async Task AggregateResultsAndCreateCsvFile(IEnumerable<string> args)
        {
            var aggregator = new TournamentAggregator();
            string fileName = $"Results_{DateTime.Now.ToLocalTime():yyyy'-'MM'-'dd'__'HH'_'mm'_'ss}.csv";
            using FileStream fs = new FileStream(fileName, FileMode.Create);

            await aggregator.AggregateResultsAndExportToCsv(args, fs);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Aggregation finished, results can be found in {fileName}");
        }
    }
}
