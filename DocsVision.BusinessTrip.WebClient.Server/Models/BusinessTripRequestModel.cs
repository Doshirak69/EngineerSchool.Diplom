using System;

namespace DocsVision.BusinessTrip.WebClient.Models
{
    public class BusinessTripRequestModel
    {
        public Guid CardId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string DateFrom { get; set; } = string.Empty;
        public string DateTo { get; set; } = string.Empty;
    }
}
