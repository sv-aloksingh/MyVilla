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

        //Custom logger 

        public VillaAPIController(ILogger<VillaAPIController> logger
            , ILogging logging
            , ApplicationDbContext dbContext)
        {
            _logger = logger;
            _logging = logging;
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.LogInformation("Getting all villas");
            _logging.Log("Getting all villas -Alok", "error"); //log info , just used error for color check.
            return Ok(_dbContext.Villas.ToList());
        }

        //[HttpGet("{id:int}")]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id <= 0)
            {
                _logger.LogError("Get Villa Error With Id" + id);
                _logging.Log("Get Villa Error With Alok Id" + id, "error");
                return BadRequest();
            }
            var villa = _dbContext.Villas.FirstOrDefault(x => x.Id == id);
            if (villa == null)
                return NotFound();
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla(VillaDTO villa)
        {
            //if (!ModelState.IsValid)   ApiController attribute take care validation from DataAnnotations so removed this
            //    return BadRequest();
            //Create Villa only if Name is unique else give custom error.
            if (_dbContext.Villas.FirstOrDefault(x => x.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError","Villa already Exists!");
                return BadRequest(ModelState);
            }
            if (villa == null)
                return BadRequest();
            if (villa.Id > 0)
                return StatusCode(StatusCodes.Status500InternalServerError);
            villa.Id = _dbContext.Villas.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            var model = new Villa()
            {
                Name = villa.Name,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
                Occupancy = villa.Occupancy,
                Details = villa.Details,
                ImageUrl = villa.ImageUrl,
                Amenity = villa.Amenity
            };
            _dbContext.Villas.Add(model);
            _dbContext.SaveChanges();
            return CreatedAtRoute("GetVilla",new { id = villa.Id},villa);
        }

        [HttpDelete("id",Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            var villa = _dbContext.Villas.FirstOrDefault(x => x.Id == id);
            if (id <= 0)
                return BadRequest();
            if (villa == null)
                return NotFound();
            _dbContext.Villas.Remove(villa);
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateVilla(int id,[FromBody]VillaDTO villa)
        {
            if (villa == null || (id != villa.Id))
                return BadRequest();
            var villas = _dbContext.Villas.FirstOrDefault(x=>x.Id == villa.Id);
            if (villas == null)
                return NotFound();
            villas.Id = villa.Id;
            villas.Name = villa.Name;
            villas.Sqft = villa.Sqft;
            villas.Occupancy = villa.Occupancy;
            _dbContext.Update(villas);
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPatch("id",Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
                return BadRequest();
            var villa = _dbContext.Villas.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (villa == null)
                return NotFound();

            VillaDTO villasDTO = new VillaDTO()
            {
                Id = villa.Id,
                Name = villa.Name,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
                Occupancy = villa.Occupancy,
                Details = villa.Details,
                ImageUrl = villa.ImageUrl,
                Amenity = villa.Amenity
            };

            patchDTO.ApplyTo(villasDTO, ModelState);

            Villa villas = new Villa
            {
                Id = villasDTO.Id,
                Name = villasDTO.Name,
                Rate = villasDTO.Rate,
                Sqft = villasDTO.Sqft,
                Occupancy = villasDTO.Occupancy,
                Details = villasDTO.Details,
                ImageUrl = villasDTO.ImageUrl,
                Amenity = villasDTO.Amenity
            };

            _dbContext.Update(villas);
            _dbContext.SaveChanges();
            if (!ModelState.IsValid)
                return BadRequest();

            return NoContent();
        }
    }
}
