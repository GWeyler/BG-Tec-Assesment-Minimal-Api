using BG_Tec_Assesment_Minimal_Api.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class TravellerDTO
    {
        public int Id { get; set; }
    
        public string Forename { get; set; }

        public string Surname { get; set; }

        public DateOnly Dob { get; set; }

        public List<FlightDTO> Flights { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
