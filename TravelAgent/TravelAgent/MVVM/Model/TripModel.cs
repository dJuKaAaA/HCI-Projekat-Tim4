using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.MVVM.Model
{
    public class TripModel
    {
        public int Id { get; set; }
        public LocationModel? Departure { get; set; }
        public LocationModel? Destination { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public double Price { get; set; }
    }
}
