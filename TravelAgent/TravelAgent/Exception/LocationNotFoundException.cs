using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Exception
{
    public class LocationNotFoundException : System.Exception
    {
        public LocationNotFoundException() 
            : base ("Location not found!") { }

        public LocationNotFoundException(string message)
            : base(message) { }
    }
}
