using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoDbContext : DbContext
    {
        public CityInfoDbContext(DbContextOptions<CityInfoDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;

        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData
                (new City("Tehran")
                {
                    Id = 1,
                    Description="This Is Tehran",
                },
                new City("Zanjan")
                {
                    Id = 2,
                    Description = "This Is Zanjan"
                },
                new City("Isfahan")
                {
                    Id = 3,
                    Description ="This Is Isfahan"
                },
                new City("Karaj")
                {
                    Id = 4,
                    Description = "This Is Karaj"
                }
                );
            modelBuilder.Entity<PointOfInterest>()
                .HasData
                ( new PointOfInterest("Milad Tower")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "This Is Milad"
                },
                new PointOfInterest("Khajoo Bridge")
                {
                    Id=2,
                    CityId = 3,
                    Description = "Khajoo Bridge"
                },
                new PointOfInterest("Chalus Road")
                {
                    Id = 3,
                    CityId = 4,
                    Description = "Road"
                },
                new PointOfInterest("Saei Park")
                {
                    Id = 4,
                    CityId = 1,
                    Description = "Park"
                }
                
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
