using System.Text.Json.Serialization;

namespace ServerExtension.Models
{
    public class TravelPayoutsFlightModel
    {
        [JsonPropertyName("flight_number")]
        public string FlightNumber { get; set; }

        [JsonPropertyName("airline")]
        public string Airline { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("departure_at")]
        public string DepartureAt { get; set; }

        [JsonPropertyName("return_at")]
        public string ReturnAt { get; set; }

        [JsonPropertyName("origin_airport")]
        public string OriginAirport { get; set; }

        [JsonPropertyName("destination_airport")]
        public string DestinationAirport { get; set; }

        [JsonPropertyName("transfers")]
        public int Transfers { get; set; }

        [JsonPropertyName("return_transfers")]
        public int ReturnTransfers { get; set; }

        [JsonPropertyName("duration_to")]
        public int DurationTo { get; set; }

        [JsonPropertyName("duration_back")]
        public int DurationBack { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
