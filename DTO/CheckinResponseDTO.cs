using BG_Tec_Assesment_Minimal_Api.Utils;
using System.ComponentModel.DataAnnotations;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class CheckinResponseDTO
    {
        public string? Status { get; set; }
        public int? TravellerId { get; set; }
    }
}
