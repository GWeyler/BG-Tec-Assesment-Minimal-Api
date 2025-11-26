using BG_Tec_Assesment_Minimal_Api.DTO;

namespace BG_Tec_Assesment_Minimal_Api.Services
{
    public interface ITravellerService
    {
        public Task<CheckinRequestResponse> CheckInTravellerAsync(CheckInRequest travellerCheckInRequest);
    }
}
