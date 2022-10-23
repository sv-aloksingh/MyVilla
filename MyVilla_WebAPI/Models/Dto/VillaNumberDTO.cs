using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Models.Dto
{
    public class VillaNumberDTO
    {
        [Required]
        public int VillaNo { get; set; }
        [Required]
        public int VillaID { get; set; }
        public string SpecialDetails { get; set; }
        public VillaDTO Villa { get; set; }
    }
}
