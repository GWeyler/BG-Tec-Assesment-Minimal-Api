using BG_Tec_Assesment_Minimal_Api.DTO;

namespace BG_Tec_Assesment_Minimal_Api.Services
{
    public interface ITravellerService
    {
        public Task<RequestResult<CheckinResponseDTO>> CheckInTravellerAsync(CheckInRequest travellerCheckInRequest);

        public Task<RequestResult<TravellerDTO>> GetTravellerByIdAsync(int travellerId);

        public Task<RequestResult<List<TravellerDTO>>> SearchTravellerAsync(TravellerSearchRequest travellerSearchRequest);
    }
}
