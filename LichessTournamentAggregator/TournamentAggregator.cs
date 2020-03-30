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

            var resultsGroupedByPlayer = results
                .Where(r => !string.IsNullOrWhiteSpace(r?.Username))
                .GroupBy(r => r.Username);

            foreach (var grouping in resultsGroupedByPlayer)
            {
                yield return new AggregatedResult(grouping);
            }
        }

        public async Task<FileStream> AggregateResultsAndExportToCsv(IEnumerable<string> tournamentIdsOrUrls, FileStream fileStream, string separator = ";")
        {
            using StreamWriter sw = new StreamWriter(fileStream);

            var headers = new List<string> { "Username", "Total Score", "Max Rating", "Ranks", "Scores", "Average Performance" };
            var lines = new List<string> { string.Join(separator, headers) };

            var internalSeparator = separator == ";" ? "," : ";";
            string aggregate(IEnumerable<int> items) => $"[{string.Join(internalSeparator, items)}]";

            await foreach (var result in AggregateResults(tournamentIdsOrUrls))
            {
                var columns = new string[] { result.Username, result.TotalScores.ToString(), result.MaxRating.ToString(), aggregate(result.Ranks), aggregate(result.Scores), result.AveragePerformance.ToString("F") };
                lines.Add(string.Join(separator, columns));
            }

            lines.ForEach(sw.WriteLine);

            return fileStream;
        }

        private IEnumerable<Uri> GetUrls(IEnumerable<string> tournamentIdsOrUrls)
        {
            const string lichessTournamentUrl = "lichess.org/tournament/";
            foreach (var item in tournamentIdsOrUrls.Select(str => str.Trim()))
            {
                string tournamentId = item;

                if (tournamentId.Contains(lichessTournamentUrl))
                {
                    var reverse = string.Join("", item.Reverse());
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
                throw new ArgumentException("The following tournament url doesn't seem exist",
                    $"{url.OriginalString.Replace("/results", string.Empty)}");
            }

            var rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var lines = rawContent.Split('\n').Where(str => !string.IsNullOrWhiteSpace(str));

            return lines.Select(line => JsonSerializer.Deserialize<TournamentResult>(line));
        }
    }
}
