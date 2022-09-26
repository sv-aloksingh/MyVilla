using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyVilla_WebAPI.Data;
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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }

        //[HttpGet("{id:int}")]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<VillaDTO> GetVillas(int id)
        {
            if (id <= 0)
                return BadRequest();
            var villa = new VillaDTO();
            if (VillaStore.villaList.Any())
                villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
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
            if (VillaStore.villaList.FirstOrDefault(x => x.Name.ToLower() == villa.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError","Villa already Exists!");
                return BadRequest(ModelState);
            }
            if (villa == null)
                return BadRequest();
            if (villa.Id > 0)
                return StatusCode(StatusCodes.Status500InternalServerError);
            villa.Id = VillaStore.villaList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villa);
            return CreatedAtRoute("GetVilla",new { id = villa.Id},villa);
        }

        [HttpDelete("id",Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (id <= 0)
                return BadRequest();
            if (villa == null)
                return NotFound();
            VillaStore.villaList.Remove(villa);
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
            var villas = VillaStore.villaList.FirstOrDefault(x=>x.Id == villa.Id);
            if (villas == null)
                return NotFound();
            villas.Id = villa.Id;
            villas.Name = villa.Name;
            villas.Sqft = villa.Sqft;
            villas.Occupancy = villa.Occupancy;
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
            var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (villa == null)
                return NotFound();

            patchDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
                return BadRequest();

            return NoContent();
        }
    }
}
