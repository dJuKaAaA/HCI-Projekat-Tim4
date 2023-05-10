using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.MVVM.Model
{
    public class RestorauntModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stars { get; set; }
        public int LocationId { get; set; }
    }
}
