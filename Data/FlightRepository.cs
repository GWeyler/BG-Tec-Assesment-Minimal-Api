using BG_Tec_Assesment_Minimal_Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BG_Tec_Assesment_Minimal_Api.Data
{

    public class FlightRepository : IGenericRepository<Flight>
    {
        private readonly TravellerAPIDbContext _context;
        public FlightRepository(TravellerAPIDbContext context)
        {
            _context = context;
        }
        public async Task<Flight?> GetEntityAsync(Expression<Func<Flight, bool>> predicate)
        {
            return await _context.Flights
                .Include(f => f.Travellers)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<Flight> AddEntityAsync(Flight flight)
        {
            var ret = await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();
            return ret.Entity;
        }
        public async Task<Flight?> GetEntityByIdAsync(int flightId)
        {
            return await _context.Flights
                .Include(f => f.Travellers)
                .SingleOrDefaultAsync(f => f.Id == flightId);
        }
        public async Task<List<Flight>> SearchEntityAsync(Expression<Func<Flight, bool>> predicate)
        {
            return await _context.Flights
                .Include(f => f.Travellers)
                .Where(predicate)
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
