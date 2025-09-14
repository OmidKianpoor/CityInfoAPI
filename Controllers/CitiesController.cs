using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Repositories;
using CityInfo.API.Services.MailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _Repository;
        private readonly IMapper _Mapper;
        private readonly IMailService _MailService;
        public CitiesController(ICityInfoRepository repository, IMapper mapper,IMailService mailService)
        {
            _Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _MailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            
        }
        [HttpGet]
        public async Task<IActionResult> GetCities()

        {
            var Cities = await _Repository.GetCitiesAsync();

            return Ok(_Mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(Cities));

           
        }
        [HttpGet("{id}",Name ="GetOneCity")]
        public async Task<IActionResult> GetOneCity(int id, bool includePointsOfInterest = false)
        {
           var city = await _Repository.GetOneCityAsync(id, includePointsOfInterest);
            if(city == null) {return NotFound();}
            if(includePointsOfInterest)
            {
                return Ok(_Mapper.Map<CityDto>(city));
            }


            return Ok(_Mapper.Map<CityWithoutPointOfInterestDto>(city));


        }


        [HttpPost]
        public async Task<IActionResult> AddNewCityAsync([FromBody]CityForCreateDto cityForCreate)
        {
           var cityToCreate = _Mapper.Map<Entities.City>(cityForCreate);

            _Repository.AddCity(cityToCreate);
           await _Repository.SaveChangesAsync();

            var createdCity = _Mapper.Map<CityWithoutPointOfInterestDto>(cityToCreate);

            return CreatedAtRoute("GetOneCity", new {id = createdCity.Id},createdCity);

        }

        [HttpPut("{cityId}")]
        public async Task<IActionResult> UpdateCity(int cityId,CityForUpdateDto cityForUpdate)
        {
            
            var city = await _Repository.GetOneCityAsync(cityId,false);

            if (city == null) return NotFound();

            _Mapper.Map(cityForUpdate, city);
            await _Repository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{cityId}")]

        public async Task<IActionResult> PartialEditCity
            (int cityId, [FromBody]JsonPatchDocument<CityForUpdateDto> patchDocument)
        {
            var city = await _Repository.GetOneCityAsync(cityId,false);
            if (city == null) return NotFound();

           var cityToPatch = _Mapper.Map<CityForUpdateDto>(city);

            patchDocument.ApplyTo(cityToPatch);

            if (!ModelState.IsValid)
            { return BadRequest(ModelState); } 

            if (!TryValidateModel(cityToPatch))
            { return BadRequest(ModelState); }

            _Mapper.Map(cityToPatch, city);

            await _Repository.SaveChangesAsync();
            return NoContent();


        }
        [HttpDelete("{cityId}")]
        public async Task<ActionResult> CityDelete(int cityId)
        {
            var city = await _Repository.GetOneCityAsync(cityId,false);
            if (city == null) return NotFound();

            _Repository.DeletCity(city);

            await _Repository.SaveChangesAsync();
            _MailService.Send("Delet City Notfication",
                $"The City With Id({cityId}) And Name ({city.Name}) Deleted!");
            return NoContent();
        }

    }
}
