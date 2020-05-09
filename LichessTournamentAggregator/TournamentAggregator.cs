using LichessTournamentAggregator.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LichessTournamentAggregator
{
    public class TournamentAggregator : ITournamentAggregator
    {
        public async IAsyncEnumerable<AggregatedResult> AggregateResults(IEnumerable<string> tournamentIdsOrUrls)
        {
            var urls = GetUrls(tournamentIdsOrUrls);
            var results = (await Task.WhenAll(urls.Select(GetTournamentResults)).ConfigureAwait(false)).SelectMany(_ => _);

            foreach (var grouping in GroupResultsByPlayer(results))
            {
                yield return new AggregatedResult(grouping);
            }
        }

        public IEnumerable<AggregatedResult> AggregateResults(IEnumerable<TournamentResult> tournamentResults)
        {
            foreach (var grouping in GroupResultsByPlayer(tournamentResults))
            {
                yield return new AggregatedResult(grouping);
            }
        }

        public async Task<FileStream> AggregateResultsAndExportToCsv(IEnumerable<string> tournamentIdsOrUrls, FileStream fileStream, string separator = ";")
        {
            var aggregatedResults = new List<AggregatedResult>();
            await foreach (var result in AggregateResults(tournamentIdsOrUrls))
            {
                aggregatedResults.Add(result);
            }

            aggregatedResults = aggregatedResults
                .OrderByDescending(r => r.TotalScores)
                .ThenByDescending(r => r.AveragePerformance)
                .ToList();

            return PopulateCsvStream(fileStream, separator, aggregatedResults);
        }

        internal IEnumerable<Uri> GetUrls(IEnumerable<string> tournamentIdsOrUrls)
        {
            const string lichessTournamentUrl = "lichess.org/tournament/";
            foreach (var item in tournamentIdsOrUrls.Select(str => str.Trim()))
            {
                string tournamentId = item.Trim(new char[] { ' ', '/', '#' });

                if (tournamentId.Contains(lichessTournamentUrl))
                {
                    var reverse = string.Join("", tournamentId.Reverse());
                    tournamentId = string.Join("", reverse.Take(reverse.IndexOf("/")).Reverse());
                }

                yield return new Uri($"https://lichess.org/api/tournament/{tournamentId}/results");
            }
        }

        private async Task<IEnumerable<TournamentResult>> GetTournamentResults(Uri url)
        {
            var client = new HttpClient();

            var response = await client.GetAsync(url).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ArgumentException("The following tournament url doesn't seem to exist",
                    url.OriginalString.Replace("/results", string.Empty));
            }

            var rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var lines = rawContent.Split('\n').Where(str => !string.IsNullOrWhiteSpace(str));

            return lines.Select(line => JsonSerializer.Deserialize<TournamentResult>(line));
        }

        private static IEnumerable<IGrouping<string, TournamentResult>> GroupResultsByPlayer(IEnumerable<TournamentResult> results)
        {
            return results
                .Where(r => !string.IsNullOrWhiteSpace(r?.Username))
                .GroupBy(r => r.Username);
        }

        private static FileStream PopulateCsvStream(FileStream fileStream, string separator, IEnumerable<AggregatedResult> aggregatedResults)
        {
            var headers = new List<string> { "#", "Username", "Total Score", "Average Performance", "Max Rating", "Title", "Ranks", "Scores" };
            using var sw = new StreamWriter(fileStream);
            sw.WriteLine(string.Join(separator, headers));

            var internalSeparator = separator == ";" ? ", " : "; ";
            string aggregate<T>(IEnumerable<T> items) => $"[{string.Join(internalSeparator, items)}]";
            for (int i = 0; i < aggregatedResults.Count(); ++i)
            {
                var result = aggregatedResults.ElementAt(i);
                var columns = new string[] { (i + 1).ToString(), result.Username, result.TotalScores.ToString(), result.AveragePerformance.ToString("F"), result.MaxRating.ToString(), result.Title, aggregate(result.Ranks), aggregate(result.Scores) };
                sw.WriteLine(string.Join(separator, columns));
            }

            return fileStream;
        }
    }
}
