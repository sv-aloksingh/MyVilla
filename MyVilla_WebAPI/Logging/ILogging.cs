using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Logging
{
    public interface ILogging
    {
        public void Log(string message, string type);
    }
}
