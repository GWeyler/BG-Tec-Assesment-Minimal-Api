using BG_Tec_Assesment_Minimal_Api.Utils;
using System.ComponentModel.DataAnnotations;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class CheckinRequestResponse
    {
        [Required]
        public ErrorEnum ErrorCode { get; set; }

        [Required]
        public string Message { get; set; }
        
        public int? TravellerId { get; set; }
    }
}
