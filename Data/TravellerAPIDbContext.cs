using Microsoft.EntityFrameworkCore;
using BG_Tec_Assesment_Minimal_Api.Models;

namespace BG_Tec_Assesment_Minimal_Api.Data
{
    public class TravellerAPIDbContext : DbContext
    {
        public TravellerAPIDbContext(DbContextOptions<TravellerAPIDbContext> options) : base(options)
        {
        }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Traveller> Travellers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Seed data for Flights to make it easier to test
            //Using int Ids but could be changed to GUIDs or strings as needed
            modelBuilder.Entity<Flight>().
                HasData(
                    new Flight { Id = 1552 },
                    new Flight { Id = 1553 },
                    new Flight { Id = 1554 }
                );

        }
    }
}
