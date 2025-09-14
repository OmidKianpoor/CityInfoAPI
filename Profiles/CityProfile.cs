using AutoMapper;


namespace CityInfo.API.Profiles
{
    public class CityProfile:Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City,Models.CityWithoutPointOfInterestDto>();
            CreateMap<Entities.City,Models.CityDto>();
            CreateMap<Models.CityForCreateDto, Entities.City>();
            CreateMap<Models.CityForUpdateDto, Entities.City>();
            CreateMap<Entities.City, Models.CityForUpdateDto > ();
        }
    }
}
