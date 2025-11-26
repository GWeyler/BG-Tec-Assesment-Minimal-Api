using BG_Tec_Assesment_Minimal_Api.Utils;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class GetTravellerByIdResponse
    {
        public ErrorEnum ErrorCode { get; set; }

        public TravellerDTO? Traveller { get; set; }
    }

}
