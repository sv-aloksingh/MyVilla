using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Models.Dto
{
    public class LoginResponseDTO
    {
        public LocalUser User { get; set; }
        public string Token { get; set; }
    }
}