using System.Text.Json.Serialization;

namespace LichessTournamentAggregator.Model
{
    public class TournamentResult
    {
        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("performance")]
        public int Performance { get; set; }
    }
}
