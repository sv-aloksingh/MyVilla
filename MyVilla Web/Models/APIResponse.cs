using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyVilla_Web.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessage = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessage { get; set; }
        public object Result { get; set; }
    }
}
