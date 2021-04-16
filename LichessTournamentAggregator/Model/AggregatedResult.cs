using System;
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
        /// Chess title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Sum of the scores of all the tournaments
        /// </summary>
        public double TotalScores { get; set; }

        /// <summary>
        /// Sum of the Tie Breaks of all the tournaments
        /// </summary>
        public double TotalTieBreaks { get; set; }

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
        public IEnumerable<double> Scores { get; set; }

        /// <summary>
        /// Tie breaks in each tournament
        /// </summary>
        public IEnumerable<double> TieBreaks { get; set; }

        /// <summary>
        /// Average player performance in the tournaments.
        /// </summary>
        public double AveragePerformance { get; set; }

        public AggregatedResult(IGrouping<string, TournamentResult> results)
        {
            Username = results.First().Username;
            Title = results.First().Title;
            MaxRating = results.Max(p => p.Rating);
            Ranks = results.Select(p => p.Rank);
            Scores = results.Select(p => CalculatePoints(p.Score));
            TieBreaks = results.Select(p => p.TieBreak);
            TotalScores = Scores.Sum();
            TotalTieBreaks = TieBreaks.Sum();
            AveragePerformance = (double)results.Select(p => p.Performance).Sum() / results.Count(p => p.Rank != 0);
        }

        /// <summary>
        /// Lichess score: https://github.com/lichess-org/api/issues/99
        /// </summary>
        /// <param name="lichessScore"></param>
        /// <returns></returns>
        private static double CalculatePoints(double lichessScore)
        {
            // Flooring to the nearest half, see https://stackoverflow.com/questions/1329426/how-do-i-round-to-the-nearest-0-5
            return Math.Floor(2 * (lichessScore / 10_000_000)) / 2;
        }
    }
}
