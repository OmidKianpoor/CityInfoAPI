using CityInfo.API.Entities;

namespace CityInfo.API.Repositories
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetOneCityAsync(int cityId,bool includePointsOfInterest);
        void AddCity(City city);
        void DeletCity(City city);
        Task<bool> CityExistsAsync(int cityId);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestAsync(int cityId,int pointOfInterestId);

        Task AddPointOfInterestForCityAsync(int cityId,PointOfInterest pointOfInterest);
        
        void DeletPointOfInterest(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();

        


        

    }
}
