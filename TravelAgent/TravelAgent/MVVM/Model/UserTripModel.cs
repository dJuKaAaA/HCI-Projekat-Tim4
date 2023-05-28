using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.MVVM.Model
{
    public class UserTripModel
    {
        public UserModel? User { get; set; }
        public TripModel? Trip { get; set; }
        public Core.TripInvoiceType Type { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
