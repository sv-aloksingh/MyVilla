using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyVilla_WebAPI.Data;
using MyVilla_WebAPI.Logging;
using MyVilla_WebAPI.Models;
using MyVilla_WebAPI.Models.Dto;
using MyVilla_WebAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VillaAPI")]  // [controller] is session and contains name of currentController can be used in place of VillaAPI
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;
        private readonly ILogging _logging;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _villaRepository;
        //private readonly ApplicationDbContext _dbContext;
        protected APIResponse _response;


        //Custom logger 

        public VillaAPIController(ILogger<VillaAPIController> logger
            , ILogging logging
            , IMapper mapper
            , IVillaRepository villaRepository)
        {
            _logger = logger;
            _logging = logging;
            _mapper = mapper;
            _villaRepository = villaRepository;
            //_dbContext = dbContext;
            this._response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Getting all villas");
                _logging.Log("Getting all villas -Alok", "error"); //log info , just used error for color check.

                IEnumerable<Villa> villaList = await _villaRepository.GetAllVillaAsync();

                _response.Result = _mapper.Map<IEnumerable<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;
        }

        //[HttpGet("{id:int}")]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogError("Get Villa Error With Id" + id);
                    _logging.Log("Get Villa Error With Alok Id" + id, "error");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return BadRequest(_response);
                }
                var villa = await _villaRepository.GetVillaAsync(x => x.Id == id);
                if (villa == null)
                    return NotFound();

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla(VillaCreateDTO createDTO)
        {
            try
            {
                //if (!ModelState.IsValid)   ApiController attribute take care validation from DataAnnotations so removed this
                //    return BadRequest();
                //Create Villa only if Name is unique else give custom error.
                if (await _villaRepository.GetVillaAsync(x => x.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already Exists!");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                    return BadRequest(createDTO);
                //if (villa.Id > 0)
                //    return StatusCode(StatusCodes.Status500InternalServerError);
                //villa.Id = _dbContext.Villas.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
                var villa = _mapper.Map<Villa>(createDTO);
                //var model = new Villa()
                //{
                //    Name = villa.Name,
                //    Rate = villa.Rate,
                //    Sqft = villa.Sqft,
                //    Occupancy = villa.Occupancy,
                //    Details = villa.Details,
                //    ImageUrl = villa.ImageUrl,
                //    Amenity = villa.Amenity
                //};
                await _villaRepository.CreateVillaAsync(villa);
                _response.Result = villa;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                var villa = await _villaRepository.GetVillaAsync(x => x.Id == id);
                if (id <= 0)
                    return BadRequest();
                if (villa == null)
                    return NotFound();
                await _villaRepository.RemoveVillaAsync(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || (id != updateDTO.Id))
                    return BadRequest();

                var model = _mapper.Map<Villa>(updateDTO);
                //villas.Id = updateDTO.Id;
                //villas.Name = updateDTO.Name;
                //villas.Sqft = updateDTO.Sqft;
                //villas.Occupancy = updateDTO.Occupancy;
                await _villaRepository.UpdateVillaAsync(model);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages =
                    new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPatch("id", Name = "UpdatePartialVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            try
            {
                if (patchDTO == null || id == 0)
                    return BadRequest();
                var villa = await _villaRepository.GetVillaAsync(x => x.Id == id, isTracked: false);
                if (villa == null)
                    return NotFound();

                var villasDTO = _mapper.Map<VillaUpdateDTO>(villa);
                //VillaUpdateDTO villasDTO = new VillaUpdateDTO()
                //{
                //    Id = villa.Id,
                //    Name = villa.Name,
                //    Rate = villa.Rate,
                //    Sqft = villa.Sqft,
                //    Occupancy = villa.Occupancy,
                //    Details = villa.Details,
                //    ImageUrl = villa.ImageUrl,
                //    Amenity = villa.Amenity
                //};

                patchDTO.ApplyTo(villasDTO, ModelState);

                var villas = _mapper.Map<Villa>(villasDTO);
                //Villa villas = new Villa
                //{
                //    Id = villasDTO.Id,
                //    Name = villasDTO.Name,
                //    Rate = villasDTO.Rate,
                //    Sqft = villasDTO.Sqft,
                //    Occupancy = villasDTO.Occupancy,
                //    Details = villasDTO.Details,
                //    ImageUrl = villasDTO.ImageUrl,
                //    Amenity = villasDTO.Amenity
                //};

                await _villaRepository.UpdateVillaAsync(villas);
                if (!ModelState.IsValid)
                    return BadRequest();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return NoContent();
        }
    }
}
