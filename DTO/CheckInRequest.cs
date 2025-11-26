using System.ComponentModel.DataAnnotations;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class CheckInRequest
    {
        [Required]
        public string Forename { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Dob { get; set; }
        [Required]
        public string DocumentNumber { get; set; }
        [Required]
        public int FlightId { get; set; }
    }
}
