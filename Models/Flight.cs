using System.ComponentModel.DataAnnotations;

namespace BG_Tec_Assesment_Minimal_Api.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }

        //Other flight details that are not used in the assessment should be included here
        public List<Traveller> Travellers { get; set; } = new List<Traveller>();

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
