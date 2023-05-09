using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Exception
{
    public class DatabaseResponseException : System.Exception
    {
        public DatabaseResponseException(string message)
            : base(message) { }
    }
}
