using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ServerExtension.Models
{
    public class TravelPayoutsResponseModel
    {
        [JsonPropertyName("data")]
        public List<TravelPayoutsFlightModel> Data { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

    }
}
