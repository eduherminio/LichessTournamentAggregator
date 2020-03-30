using System.Collections.Generic;
using System.Linq;

namespace LichessTournamentAggregator.Model
{
    public class AggregatedResult
    {
        /// <summary>
        /// Lichess username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Sum of the scores of all the tournaments
        /// </summary>
        public int TotalScores { get; set; }

        /// <summary>
        /// Maximum rating while playing in the tournaments
        /// </summary>
        public double MaxRating { get; set; }

        /// <summary>
        /// Rank in each tournament
        /// </summary>
        public IEnumerable<int> Ranks { get; set; }

        /// <summary>
        /// Score in each tournament
        /// </summary>
        public IEnumerable<int> Scores { get; set; }

        /// <summary>
        /// Average player performance in the tournaments.
        /// </summary>
        public double AveragePerformance { get; set; }

        public AggregatedResult(IGrouping<string, TournamentResult> results)
        {
            Username = results.First().Username;
            MaxRating = results.Max(p => p.Rating);
            Ranks = results.Select(p => p.Rank);
            Scores = results.Select(p => p.Score);
            TotalScores = results.Select(p => p.Score).Sum();
            AveragePerformance = (double)results.Select(p => p.Performance).Sum() / results.Count(p => p.Rank != 0);
        }
    }
}
