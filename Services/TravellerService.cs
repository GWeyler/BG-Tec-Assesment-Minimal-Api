using BG_Tec_Assesment_Minimal_Api.Data;
using BG_Tec_Assesment_Minimal_Api.DTO;
using BG_Tec_Assesment_Minimal_Api.Models;
using BG_Tec_Assesment_Minimal_Api.Utils;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Text.Json;

namespace BG_Tec_Assesment_Minimal_Api.Services
{
    public class TravellerService : ITravellerService
    {


        private readonly ILogger<TravellerService> _logger;

        private readonly TravellerAPIDbContext _context;

        private readonly IMapper _mapper;

        public TravellerService(ILogger<TravellerService> logger, TravellerAPIDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<CheckinResponse> CheckInTravellerAsync(CheckInRequest travellerCheckInRequest)
        {
            Flight flight = await _context.Flights
                .Include(f => f.Travellers)
                .SingleOrDefaultAsync(f => f.Id == travellerCheckInRequest.FlightId);

            if (flight == null)
            {
                _logger.LogWarning("Flight with ID {FlightId} not found.", travellerCheckInRequest.FlightId);
                return new CheckinResponse { ErrorCode = ErrorEnum.NotFound, Message = " No flight found that matches ID" };
            }

            string documentNumberSHA = DocumentNumberSHA256Hasher.ComputeSHA256Hash(travellerCheckInRequest.DocumentNumber);

            DateOnly tmpDob;

            if (!DateOnly.TryParse(travellerCheckInRequest.Dob, out tmpDob))
            {
                _logger.LogError(" Traveller DoB could not be parsed returning BadRequest DoB: {@newCheckingRequest.Dob}", travellerCheckInRequest.Dob);
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
                _logger.LogError("Traveller validation failed returning bad request Validation results: {validationResults}", JsonSerializer.Serialize(validationResults));
                return new CheckinResponse { ErrorCode = ErrorEnum.BadRequest, Message = "Traveller Validation Failed" };
            }

            //Do we already have a passenger with that Document in the DB?
            Traveller existingTraveller = await _context.Travellers.SingleOrDefaultAsync(t => t.DocumentNumberSHA == documentNumberSHA);

            if (existingTraveller != null)
            {
                if (flight.Travellers.Contains(existingTraveller))
                {
                    _logger.LogWarning("Traveller already checked in for this flight: {@existingTraveller}", existingTraveller);
                    return new CheckinResponse { ErrorCode = ErrorEnum.DuplicateEntry, Message = "Document already used for this flight." };
                }
                else
                {
                    //TODO There is a opening for this existing traveller and new traveller to not be the same person
                    //need to check that here
                    try
                    {
                        flight.Travellers.Add(existingTraveller);
                        await _context.SaveChangesAsync();
                        return new CheckinResponse { ErrorCode = ErrorEnum.None, TravellerId = existingTraveller.Id, Message = "Accepted" };
                    }
                    catch (Exception e)
                    {
                        // _logger.LogError("{id}", HttpContext.TraceIdentifier);
                        return new CheckinResponse { ErrorCode = ErrorEnum.InternalServerError, Message = "Internal Server Error" };
                    }
                }
            }

            try
            {
                _logger.LogInformation("Adding new traveller {@t}", newTraveller);
                var ret = await _context.Travellers.AddAsync(newTraveller);
                flight.Travellers.Add(ret.Entity);
                await _context.SaveChangesAsync();
                return new CheckinResponse { ErrorCode = ErrorEnum.None, TravellerId = ret.Entity.Id, Message = "Accepted" };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving new traveller: {@ex}", ex);
                return new CheckinResponse { ErrorCode = ErrorEnum.NotFound, Message = " No flight found that matches ID" };

            }

        }
        async public Task<GetTravellerByIdResponse> GetTravellerByIdAsync(int travellerId)
        {
            Traveller traveller;
            try
            {
                traveller = await _context.Travellers
                    .Include(t => t.Flights)
                    .SingleOrDefaultAsync(t => t.Id == travellerId);
            }
            catch (Exception e)
            {
                _logger.LogError(" Issue retrieving data from DB {@e}", e);
                return new GetTravellerByIdResponse { ErrorCode = ErrorEnum.InternalServerError, Traveller = null };
            }

            return new GetTravellerByIdResponse
            {
                ErrorCode = traveller == null ? ErrorEnum.NotFound : ErrorEnum.None,
                Traveller = traveller == null ? null : _mapper.Map<TravellerDTO>(traveller)
            };
        }

    }
}

