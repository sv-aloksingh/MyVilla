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
        public IEnumerable<VillaDTO> GetVillas()
        {
            return VillaStore.villaList;
        }

        [HttpGet("id")]
        //[HttpGet("{id:int}")]
        public VillaDTO GetVillas(int id)
        {
            var villaList = VillaStore.villaList;
            if (villaList.Any())
                return villaList.FirstOrDefault(x => x.Id == id);
            return new VillaDTO();
        }
    }
}
