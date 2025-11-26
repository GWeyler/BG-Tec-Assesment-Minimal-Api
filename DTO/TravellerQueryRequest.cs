using Microsoft.AspNetCore.Mvc;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class TravellerQueryRequest
    {
        public int? FlightId { get; set; }
        public string? Name { get; set; }
        public string? Dob_to { get; set; }
        public string? Dob_from { get; set; }

    }
}
