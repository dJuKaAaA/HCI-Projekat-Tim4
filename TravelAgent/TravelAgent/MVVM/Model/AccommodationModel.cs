using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.MVVM.Model
{
    public class AccommodationModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Rating { get; set; }
        public LocationModel? Location { get; set; }
        public string? Image { get; set; }
    }
}
