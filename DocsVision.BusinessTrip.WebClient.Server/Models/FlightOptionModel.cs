using System.Text.Json.Serialization;

namespace ServerExtension.Models
{
    public class FlightOptionModel
    {
        [JsonPropertyName("airline")]
        public string Airline { get; set; }

        [JsonPropertyName("flightNumber")]
        public string FlightNumber { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("departureAt")]
        public string DepartureAt { get; set; }

        [JsonPropertyName("returnAt")]
        public string ReturnAt { get; set; }

        [JsonPropertyName("originAirport")]
        public string OriginAirport { get; set; }

        [JsonPropertyName("destinationAirport")]
        public string DestinationAirport { get; set; }

        [JsonPropertyName("transfers")]
        public int Transfers { get; set; }

        [JsonPropertyName("returnTransfers")]
        public int ReturnTransfers { get; set; }

        [JsonPropertyName("durationTo")]
        public int DurationTo { get; set; }

        [JsonPropertyName("durationBack")]
        public int DurationBack { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
