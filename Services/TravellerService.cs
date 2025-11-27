using BG_Tec_Assesment_Minimal_Api.Data;
using BG_Tec_Assesment_Minimal_Api.DTO;
using BG_Tec_Assesment_Minimal_Api.Models;
using BG_Tec_Assesment_Minimal_Api.Utils;
using LinqKit;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text.Json;

namespace BG_Tec_Assesment_Minimal_Api.Services
{
    public class TravellerService : ITravellerService
    {


        private readonly ILogger<TravellerService> _logger;

        private readonly IMapper _mapper;

        private readonly IGenericRepository<Flight> _flightRepository;

        private readonly IGenericRepository<Traveller> _travellerRepository; 

        private readonly string DATE_FORMAT = "yyyy-MM-dd"; 
        public TravellerService(ILogger<TravellerService> logger, IMapper mapper, IGenericRepository<Flight> flightRepository, IGenericRepository<Traveller> travellerRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _flightRepository = flightRepository;
            _travellerRepository = travellerRepository;

        }

        public async Task<CheckinResponse> CheckInTravellerAsync(CheckInRequest travellerCheckInRequest)
        {
            _logger.LogDebug("CheckInTravellerAsync called with {@travellerCheckInRequest}", travellerCheckInRequest);
            Flight? flight;
            try
            {
               flight = await _flightRepository.GetEntityAsync(f => f.Id == travellerCheckInRequest.FlightId);

            }catch (Exception e)
            {
                _logger.LogError("Issue retrieving data from DB {@e}", e);
                return new CheckinResponse { ErrorCode = ErrorEnum.InternalServerError, Message = "Internal Server Error" };
            }

            if (flight == null)
            {
                _logger.LogInformation("Flight with ID {FlightId} not found", travellerCheckInRequest.FlightId);
                return new CheckinResponse { ErrorCode = ErrorEnum.NotFound, Message = "No flight found that matches ID" };
            }

            
            string documentNumberSHA = DocumentNumberSHA256Hasher.ComputeSHA256Hash(travellerCheckInRequest.DocumentNumber);

            DateOnly tmpDob;

            if (!DateOnly.TryParseExact(travellerCheckInRequest.Dob, DATE_FORMAT, null, System.Globalization.DateTimeStyles.None, out tmpDob))
            {
                _logger.LogInformation("Traveller DoB could not be parsed returning BadRequest DoB: {@newCheckingRequest.Dob}", travellerCheckInRequest.Dob);
                return new CheckinResponse { ErrorCode = ErrorEnum.BadRequest, Message = " Dob could not be parsed" };
            }

            Traveller newTraveller = new Traveller
            {
                Forename = travellerCheckInRequest.Forename,
                Surname = travellerCheckInRequest.Surname,
                Dob = tmpDob,
                CreatedAt = DateTime.Now.ToUniversalTime(),
                DocumentNumber = travellerCheckInRequest.DocumentNumber,
                DocumentNumberSHA = documentNumberSHA.ToString(),
            };


            var validationContext = new ValidationContext(newTraveller);
            var validationResults = new List<ValidationResult>();


            if (!Validator.TryValidateObject(newTraveller, validationContext, validationResults, true))
            {
                _logger.LogInformation("Traveller validation failed returning bad request Validation results: {validationResults}", JsonSerializer.Serialize(validationResults));
                return new CheckinResponse { ErrorCode = ErrorEnum.BadRequest, Message = "Traveller Validation Failed "+ JsonSerializer.Serialize(validationResults) };
            }

            //Do we already have a passenger with that Document in the DB?
            Traveller? existingTraveller;
            try
            {
               existingTraveller  = await _travellerRepository.GetEntityAsync(t => t.DocumentNumberSHA == documentNumberSHA);

            } catch (Exception e)
            {
                _logger.LogError(" Issue retrieving data from DB {@e}", e);
                return new CheckinResponse { ErrorCode = ErrorEnum.InternalServerError, Message = "Internal Server Error" };
            }

            if (existingTraveller != null)
            {
                _logger.LogDebug("Existing traveller found {@existingTraveller}", existingTraveller);
                if (flight.Travellers.Contains(existingTraveller))
                {
                    _logger.LogWarning("Traveller already checked in for this flight: {@existingTraveller}", existingTraveller);
                    return new CheckinResponse { ErrorCode = ErrorEnum.DuplicateEntry, Message = "Document already used for this flight." };
                }
                else
                {
                    if (!existingTraveller.SoftEquals(newTraveller))
                    {
                        _logger.LogWarning("Existing traveller does not match new traveller details {@existingTraveller} {@newTraveller}", existingTraveller, newTraveller);
                        return new CheckinResponse { ErrorCode = ErrorEnum.BadRequest, Message = "Document matches existing traveller but details do not match." };
                    }

                    try
                    {
                        _logger.LogInformation("Existing Traveller {id} being add to flight {id}",existingTraveller.Id, flight.Id);
                        flight.Travellers.Add(existingTraveller);
                        await _flightRepository.SaveChangesAsync();
                        return new CheckinResponse { ErrorCode = ErrorEnum.None, TravellerId = existingTraveller.Id, Message = "Accepted" };
                    }
                    catch (Exception e)
                    {
                         _logger.LogError("Exception Existing Traveller to flight {e}",e);
                        return new CheckinResponse { ErrorCode = ErrorEnum.InternalServerError, Message = "Internal Server Error" };
                    }
                }
            }

            try
            {
                _logger.LogInformation("Adding new traveller {@t} to db and flight {fid}", newTraveller,flight.Id);
                var ret = await _travellerRepository.AddEntityAsync(newTraveller);
                flight.Travellers.Add(ret);
                await _flightRepository.SaveChangesAsync();
                return new CheckinResponse { ErrorCode = ErrorEnum.None, TravellerId = ret.Id, Message = "Accepted" };
            }
            catch (Exception e)
            {
                _logger.LogError("Error saving new traveller: {@t} {@e}",newTraveller, e);
                return new CheckinResponse { ErrorCode = ErrorEnum.InternalServerError, Message = "Internal Server Error" };

            }

        }
        async public Task<GetTravellerByIdResponse> GetTravellerByIdAsync(int travellerId)
        {
            _logger.LogDebug("GetTravellerByIdAsync called with ID: {travellerId}", travellerId);
            Traveller? traveller;
            try
            {
                traveller = await _travellerRepository.GetEntityByIdAsync(travellerId); 
            }
            catch (Exception e)
            {
                _logger.LogError("Issue retrieving data from DB {@e}", e);
                return new GetTravellerByIdResponse { ErrorCode = ErrorEnum.InternalServerError, Traveller = null };
            }

            return new GetTravellerByIdResponse
            {
                ErrorCode = traveller == null ? ErrorEnum.NotFound : ErrorEnum.None,
                Traveller = traveller == null ? null : _mapper.Map<TravellerDTO>(traveller)
            };
        }
        async public Task<TravellerSearchResponse> SearchTravellerAsync(TravellerSearchRequest travellerSearchRequest)
        {
            var predicate = PredicateBuilder.New<Traveller>();
            _logger.LogDebug("SearchTravellerAsync Called with {@TravellerSearchRequest}",travellerSearchRequest);

            if (travellerSearchRequest.FlightId != null)
            {
                predicate = predicate.And(t => t.Flights.Any(f => f.Id == travellerSearchRequest.FlightId));
            }

            if (!string.IsNullOrEmpty(travellerSearchRequest.Name))
            {
                predicate = predicate.And(t => t.Forename.Contains(travellerSearchRequest.Name) || t.Surname.Contains(travellerSearchRequest.Name));
            }

            if (!string.IsNullOrEmpty(travellerSearchRequest.Dob_from))
            {
                if (DateOnly.TryParseExact(travellerSearchRequest.Dob_from,DATE_FORMAT,null,System.Globalization.DateTimeStyles.None, out DateOnly dobFromDate))
                {
                    predicate = predicate.And(t => t.Dob >= dobFromDate);
                }
                else
                {
                    _logger.LogInformation("Bad Date Format on Dob_From {dob_from}", travellerSearchRequest.Dob_from);
                    return new TravellerSearchResponse
                    {
                        ErrorCode = ErrorEnum.BadRequest,
                        Messsage = "Bad Date Format on Dob_From"
                    };
                }
            }

            if (!string.IsNullOrEmpty(travellerSearchRequest.Dob_to))
            {
                if (DateOnly.TryParseExact(travellerSearchRequest.Dob_from, DATE_FORMAT, null, System.Globalization.DateTimeStyles.None, out DateOnly dobToDate))
                {
                    predicate = predicate.And(t => t.Dob <= dobToDate);
                }
                else
                {
                    _logger.LogInformation("Bad Date Format on Dob_From {dob_from}", travellerSearchRequest.Dob_from);
                    return new TravellerSearchResponse
                    {
                        ErrorCode = ErrorEnum.BadRequest,
                        Messsage ="Bad Date Format on Dob_to"
                    };
                }
            }

            List<Traveller> travellers;
            try
            {
                 travellers = await _travellerRepository.SearchEntityAsync(predicate);
            }
            catch (Exception e)
            {
                _logger.LogError("Issue retrieving data from DB {@e}", e);
                return new TravellerSearchResponse { ErrorCode = ErrorEnum.InternalServerError, Messsage="Error getting data from DB" };
            }

            return new TravellerSearchResponse
            {
                ErrorCode = ErrorEnum.None,
                Travellers = travellers.IsNullOrEmpty()? new List<TravellerDTO>(): travellers.Select(t => t.Adapt<TravellerDTO>()).ToList()
            };

        }

    }
}

