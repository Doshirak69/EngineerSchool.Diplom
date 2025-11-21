using System;

namespace DocsVision.BusinessTrip.WebClient.Models
{
    public class EmployeeChangeRequestModel
    {
        public Guid CardId { get; set; }
        public string EmployeeAccount { get; set; } = string.Empty;
    }
}