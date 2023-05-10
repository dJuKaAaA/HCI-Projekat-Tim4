using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.MVVM.Model
{
    public class FlightModel
    {
        public int Id { get; set; }
        public LocationModel Departure { get; set; }
        public LocationModel Destination { get; set; }
        public DateTime TakeoffDateTime { get; set; }
        public DateTime LandingDateTime { get; set; }
        public double Price { get; set; }
    }
}
