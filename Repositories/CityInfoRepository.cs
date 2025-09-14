using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Repositories
{
    public class CityInfoRepository : ICityInfoRepository
    {
        public CityInfoDbContext _context { get; set; }

        public CityInfoRepository(CityInfoDbContext context)
        {
            _context = context ?? throw new ArgumentException(nameof(context));
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(p => p.Id).ToListAsync();
        }

        public async Task<City?> GetOneCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities
                    .Include(c => c.PointOfInterest).Where(p => p.Id == cityId).FirstOrDefaultAsync();
            }
            return await _context.Cities.Where(p => p.Id == cityId).FirstOrDefaultAsync(); 
        }

        public void AddCity(City city)
        {
            _context.Cities.Add(city);
        }

        public void DeletCity(City city)
        {
            _context.Cities.Remove(city);
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(p => p.Id == cityId);
        }
        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId)
        {
            
            return await _context.PointsOfInterest.Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId && p.Id == pointOfInterestId).FirstOrDefaultAsync();
        }



        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetOneCityAsync(cityId,false);
            if (city != null) 
            {
                city.PointOfInterest.Add(pointOfInterest);
            } 
        }

        public void DeletPointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }




       



    }
}
