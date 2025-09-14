
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Repositories;
using CityInfo.API.Services.MailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cities/{cityId}/pointOfInterests")]
    public class PointOfInterestsController : ControllerBase
    {
        private readonly ILogger<PointOfInterestsController> _logger;
        private readonly IMailService _localMailService;
       
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        public PointOfInterestsController
            (ILogger<PointOfInterestsController> logger, IMailService localMailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _localMailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        #region Show All PoinrOfInterests
        public async Task<IActionResult> GetPointsOfInterest(int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"Cannot Found City With {cityId} Id ! ");
                return NotFound();
            }
            var pointOfInterestsForCity = await _cityInfoRepository.GetPointsOfInterestAsync(cityId);
         
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestsForCity));

        }
        #endregion 

        [HttpGet("{pointOfInterestId}", Name = "GetOnePointOfInterest")]
        #region Show One PointOfInterest
        public async Task<ActionResult<PointOfInterestDto>> GetOnePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"Cannot Find City With {cityId} Id ! ");
                return NotFound();
            }
            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestAsync(cityId, pointOfInterestId);
            if (pointOfInterest == null)
            {
                _logger.LogInformation($"This City Have Not PointOfInterest With {pointOfInterestId} Id ");
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));

        }
        #endregion

        [HttpPost]
        #region CreatePointOfInterest
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest
            (int cityId, [FromBody] PointOfInterestForCreateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var finalPoint = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPoint);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPoint = _mapper.Map<Models.PointOfInterestDto>(finalPoint);

            return CreatedAtRoute("GetOnePointOfInterest",
                new { cityId = cityId, pointOfInterestId = createdPoint.Id }
               , createdPoint);
        }
        #endregion

        [HttpPut("{PointOfInterestId}")]
        #region EditPointOfInterest
        public async Task<ActionResult> UpdatePointOfInterest
            (int cityId, int PointOfInterestId, [FromBody] PointOfInterestForUpdateDto pointOfInterestForUpdate)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }
            var point = await _cityInfoRepository.GetPointOfInterestAsync(cityId, PointOfInterestId);
            if (point == null) { return NotFound(); }

            _mapper.Map(pointOfInterestForUpdate, point);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();

          
        }
        #endregion


        [HttpPatch("{PointOfInterestId}")]
        #region EditOneLineOfOnePointOfInterest(With Patch)
        public async Task<ActionResult> PartialUpdateOfPointOfInterest
            (int cityId, int PointOfInterestId,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointEntities = await _cityInfoRepository.GetPointOfInterestAsync(cityId, PointOfInterestId);
            if (pointEntities == null) {return NotFound();}

            var pointToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointEntities);

            patchDocument.ApplyTo(pointToPatch,ModelState);

            if(!ModelState.IsValid)
            { return BadRequest(ModelState); }  

            if(!TryValidateModel(pointToPatch))
                { return BadRequest(ModelState); } 

            _mapper.Map(pointToPatch,pointEntities);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        [HttpDelete("{PointOfInterestId}")]
        #region Delete
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int PointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointEntities = await _cityInfoRepository.GetPointOfInterestAsync(cityId, PointOfInterestId);
            if (pointEntities == null) { return NotFound(); }

             _cityInfoRepository.DeletPointOfInterest(pointEntities);
             await _cityInfoRepository.SaveChangesAsync();
            _localMailService.Send
                ("Delet Notfication", $" {pointEntities.Name}({pointEntities.Id}) Deleted!");

            return NoContent();

        }
        #endregion
    }
}
