using MyVilla_WebAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Data
{
    public static class VillaStore
    {
        public static List<VillaDTO> villaList = new List<VillaDTO>()
            {
                new VillaDTO {Id = 1, Name = "Pool View"},
                new VillaDTO {Id = 2, Name = "Beach View"}
            };
    }
}

