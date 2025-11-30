using System.ComponentModel.DataAnnotations;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class CheckInRequest
    {
        [Required]
        public required  string Forename { get; set; }
        [Required]
        public required string Surname { get; set; }
        [Required]
        public required string Dob { get; set; }
        [Required]
        public required  string DocumentNumber { get; set; }
        [Required]
        public int FlightId { get; set; }
    }
}
