using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyVilla_WebAPI.Logging
{
    public class Loggings : ILogging
    {
        public void Log(string message, string type)
        {
            if (type == "error")
            {
                Console.WriteLine("Error - " + message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}
