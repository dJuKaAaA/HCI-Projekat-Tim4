using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.MVVM.Model
{
    public class UserFlightModel
    {
        public UserModel? User { get; set; }
        public FlightModel? Flight { get; set; }
        public Core.FlightInvoiceType Type { get; set; }
    }
}
