using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BG_Tec_Assesment_Minimal_Api.Models;
using System.Formats.Asn1;

namespace BG_Tec_Assesment_Minimal_Api.Data
{
    public class TravellerRepository : IGenericRepository<Traveller>
    {    
        TravellerAPIDbContext _context;

        public TravellerRepository(TravellerAPIDbContext context)
        {
            _context = context;

        }

        public async Task<Traveller?> GetEntityAsync(Expression<Func<Traveller, bool>> predicate)
        {
            return await _context.Travellers
                .Include(t => t.Flights)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<Traveller> AddEntityAsync(Traveller traveller)
        {
            var ret = await _context.Travellers.AddAsync(traveller);
            await _context.SaveChangesAsync();
            return ret.Entity;  
        }

        public async Task<Traveller?> GetEntityByIdAsync(int travellerId)
        {
            return await _context.Travellers
                .Include(t => t.Flights)
                .SingleOrDefaultAsync(t => t.Id == travellerId);
        }

        public async Task<List<Traveller>> SearchEntityAsync(Expression<Func<Traveller, bool>> predicate)
        {
            return await _context.Travellers
                .Include(t => t.Flights)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
