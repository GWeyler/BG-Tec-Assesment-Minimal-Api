using BG_Tec_Assesment_Minimal_Api.Utils;
using System.Globalization;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class TravellerSearchResponse
    {
        public ErrorEnum ErrorCode { get; set; }

        public List<TravellerDTO> Travellers { get; set; } = new List<TravellerDTO>();

        public string? Messsage { get; set; }
    }
}
