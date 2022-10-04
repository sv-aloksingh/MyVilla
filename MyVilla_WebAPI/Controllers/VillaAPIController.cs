using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyVilla_WebAPI.Data;
using MyVilla_WebAPI.Logging;
using MyVilla_WebAPI.Models;
using MyVilla_WebAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        //Custom logger 

        public VillaAPIController(ILogger<VillaAPIController> logger
            , ILogging logging
            , IMapper mapper
            , ApplicationDbContext dbContext)
        {
            _logger = logger;
            _logging = logging;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.LogInformation("Getting all villas");
            _logging.Log("Getting all villas -Alok", "error"); //log info , just used error for color check.

            IEnumerable<Villa> villaList = await _dbContext.Villas.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDTO>>(villaList));
        }

        //[HttpGet("{id:int}")]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Get Villa Error With Id" + id);
                _logging.Log("Get Villa Error With Alok Id" + id, "error");
                return BadRequest();
            }
            var villa = await _dbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);
            if (villa == null)
                return NotFound();
            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla(VillaCreateDTO createDTO)
        {
            //if (!ModelState.IsValid)   ApiController attribute take care validation from DataAnnotations so removed this
            //    return BadRequest();
            //Create Villa only if Name is unique else give custom error.
            if (await _dbContext.Villas.FirstOrDefaultAsync(x => x.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError","Villa already Exists!");
                return BadRequest(ModelState);
            }
            if (createDTO == null)
                return BadRequest(createDTO); 
            //if (villa.Id > 0)
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //villa.Id = _dbContext.Villas.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            var model = _mapper.Map<Villa>(createDTO);
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
            await _dbContext.Villas.AddAsync(model);
            await _dbContext.SaveChangesAsync();
            return CreatedAtRoute("GetVilla",new { id = model.Id},model);
        }

        [HttpDelete("id",Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            var villa = await _dbContext.Villas.FirstOrDefaultAsync(x => x.Id == id);
            if (id <= 0)
                return BadRequest();
            if (villa == null)
                return NotFound();
            _dbContext.Villas.Remove(villa);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVilla(int id,[FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || (id != updateDTO.Id))
                return BadRequest();

            var model = _mapper.Map<Villa>(updateDTO);
            //villas.Id = updateDTO.Id;
            //villas.Name = updateDTO.Name;
            //villas.Sqft = updateDTO.Sqft;
            //villas.Occupancy = updateDTO.Occupancy;
            _dbContext.Update(model);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("id",Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
                return BadRequest();
            var villa = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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

            _dbContext.Update(villas);
            await _dbContext.SaveChangesAsync();
            if (!ModelState.IsValid)
                return BadRequest();

            return NoContent();
        }
    }
}
