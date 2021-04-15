using LichessTournamentAggregator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LichessTournamentAggregator.Test
{
    public class AggregatedResultTest
    {
        private const string Username = "OurUser";

        [Fact]
        public void MaxRating()
        {
            ICollection<TournamentResult> results = new[]
            {
                new TournamentResult()
                {
                    Username = Username,
                    Rating = 1002
                },
                new TournamentResult()
                {
                    Username = Username,
                    Rating = 900
                },
                new TournamentResult()
                {
                    Username = Username,
                    Rating = 1300
                }
            };

            AggregatedResult aggregatedResult = new AggregatedResult(results.GroupBy(r => r.Username).Single());

            Assert.Equal(results.Max(r => r.Rating), aggregatedResult.MaxRating);
        }

        [Fact]
        public void TotalScores()
        {
            ICollection<TournamentResult> results = new[]
            {
                new TournamentResult()
                {
                    Username = Username,
                    Score = 0
                },
                new TournamentResult()
                {
                    Username = Username,
                    Score = 15
                },
                new TournamentResult()
                {
                    Username = Username,
                    Score = 5
                },
                new TournamentResult()
                {
                    Username = Username,
                },
                new TournamentResult()
                {
                    Username = Guid.NewGuid().ToString(),
                    Score = 7
                },
            };

            AggregatedResult aggregatedResult = new AggregatedResult(results.GroupBy(r => r.Username).Single(g => g.Key == Username));

            Assert.Equal(results.Where(r => r.Username == Username).Sum(r => r.Score), aggregatedResult.TotalScores);
        }

        [Fact]
        public void Scores()
        {
            ICollection<TournamentResult> results = new[]
            {
                new TournamentResult()
                {
                    Username = Username,
                    Score = 0
                },
                new TournamentResult()
                {
                    Username = Username,
                    Score = 15
                },
                new TournamentResult()
                {
                    Username = Username,
                    Score = 5
                },
                new TournamentResult()
                {
                    Username = Guid.NewGuid().ToString(),
                    Score = 7
                },
            };

            AggregatedResult aggregatedResult = new AggregatedResult(results.GroupBy(r => r.Username).Single(g => g.Key == Username));

            foreach (var score in aggregatedResult.Scores)
            {
                Assert.Single(results, (result) => result.Username == aggregatedResult.Username && result.Score == score);
            }
        }

        [Fact]
        public void TotalTieBreaks()
        {
            ICollection<TournamentResult> results = new[]
            {
                new TournamentResult()
                {
                    Username = Username,
                    TieBreak = 0
                },
                new TournamentResult()
                {
                    Username = Username,
                    TieBreak = 15
                },
                new TournamentResult()
                {
                    Username = Username,
                    TieBreak = 5
                },
                new TournamentResult()
                {
                    Username = Username,
                },
                new TournamentResult()
                {
                    Username = Guid.NewGuid().ToString(),
                    TieBreak = 7
                },
            };

            AggregatedResult aggregatedResult = new AggregatedResult(results.GroupBy(r => r.Username).Single(g => g.Key == Username));

            Assert.Equal(results.Where(r => r.Username == Username).Sum(r => r.TieBreak), aggregatedResult.TotalTieBreaks);
        }

        [Fact]
        public void TieBreaks()
        {
            ICollection<TournamentResult> results = new[]
            {
                new TournamentResult()
                {
                    Username = Username,
                    TieBreak = 0
                },
                new TournamentResult()
                {
                    Username = Username,
                    TieBreak = 15
                },
                new TournamentResult()
                {
                    Username = Username,
                    TieBreak = 5
                },
                new TournamentResult()
                {
                    Username = Guid.NewGuid().ToString(),
                    TieBreak = 7
                },
            };

            AggregatedResult aggregatedResult = new AggregatedResult(results.GroupBy(r => r.Username).Single(g => g.Key == Username));

            foreach (var tieBreak in aggregatedResult.TieBreaks)
            {
                Assert.Single(results, (result) => result.Username == aggregatedResult.Username && result.TieBreak == tieBreak);
            }
        }

        [Fact]
        public void Ranks()
        {
            ICollection<TournamentResult> results = new[]
            {
                new TournamentResult()
                {
                    Username = Username,
                },
                new TournamentResult()
                {
                    Username = Username,
                    Rank = 15
                },
                new TournamentResult()
                {
                    Username = Username,
                    Rank = 5
                },
                new TournamentResult()
                {
                    Username = Guid.NewGuid().ToString(),
                    Rank = 7
                },
            };

            AggregatedResult aggregatedResult = new AggregatedResult(results.GroupBy(r => r.Username).Single(g => g.Key == Username));

            foreach (var rank in aggregatedResult.Ranks)
            {
                Assert.Single(results, (result) => result.Username == aggregatedResult.Username && result.Rank == rank);
            }
        }

        [Fact]
        public void AveragePerformance()
        {
            ICollection<TournamentResult> results = new[]
            {
                new TournamentResult()
                {
                    Username = Username,
                    Rank = 0
                },
                new TournamentResult()
                {
                    Username = Username,
                    Performance = 2801,
                    Rank = 1
                },
                new TournamentResult()
                {
                    Username = Username,
                    Performance = 2401,
                    Rank = 3
                },
                new TournamentResult()
                {
                    Username = Username,
                    Performance = 2201,
                    Rank = 6
                },
                new TournamentResult()
                {
                    Username = Guid.NewGuid().ToString(),
                    Performance = 850,
                    Rank = 1601
                },
            };

            AggregatedResult aggregatedResult = new AggregatedResult(results.GroupBy(r => r.Username).Single(g => g.Key == Username));

            Assert.Equal(
                (double)results.Where(r => r.Username == Username).Sum(r => r.Performance) / results.Count(r => r.Username == Username && r.Rank != 0),
                aggregatedResult.AveragePerformance);
        }

        [Fact]
        public void Title()
        {
            ICollection<TournamentResult> results = new[]
            {
                new TournamentResult()
                {
                    Username = Username,
                    Performance = 2801,
                    Rank = 2,
                    Title = "LM"
                },
                new TournamentResult()
                {
                    Username = Username,
                    Performance = 2401,
                    Rank = 2,
                    Title = "LM"
                },
                new TournamentResult()
                {
                    Username = Username,
                    Performance = 2201,
                    Rank = 3,
                    Title = "LM"
                },
                new TournamentResult()
                {
                    Username = Guid.NewGuid().ToString(),
                    Performance = 3333,
                    Rank = 1,
                    Title = "GM"
                }
            };

            AggregatedResult aggregatedResult = new AggregatedResult(results.GroupBy(r => r.Username).Single(g => g.Key == Username));

            Assert.Equal(
                results.Where(r => r.Username == Username).FirstOrDefault(r => !string.IsNullOrEmpty(r.Title))?.Title,
                aggregatedResult.Title);
        }
    }
}
