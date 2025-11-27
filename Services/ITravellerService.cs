using BG_Tec_Assesment_Minimal_Api.DTO;

namespace BG_Tec_Assesment_Minimal_Api.Services
{
    public interface ITravellerService
    {
        public Task<CheckinResponse> CheckInTravellerAsync(CheckInRequest travellerCheckInRequest);

        public Task<GetTravellerByIdResponse> GetTravellerByIdAsync(int travellerId);

        public Task<TravellerSearchResponse> SearchTravellerAsync(TravellerSearchRequest travellerSearchRequest);
    }
}
