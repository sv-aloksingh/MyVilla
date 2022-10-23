using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyVilla_Web.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_Web.Models.VM
{
    public class VillaNumberUpdateVM
    {
        public VillaNumberUpdateVM()
        {
            VillaNumber = new VillaNumberUpdateDTO();
        }
        public VillaNumberUpdateDTO VillaNumber { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> VillaList { get; set; }
    }
}
