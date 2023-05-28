using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Core
{
    public enum TripSearchType
    {
        Departure,
        Destination,
        DepartureDateTime,
        ArrivalDateTime,
        Price
    }

    public enum RestorauntSearchType
    {
        Name,
        Stars,
        Address,
    }

    public enum TouristAttractionSearchType
    {
        Name,
        Address
    }

    public enum AccommodationSearchType
    {
        Name,
        Rating,
        Address
    }

    public enum UserTripSearchType
    {
        Departure,
        Destination,
        DepartureDateTime,
        ArrivalDateTime,
        Price,
        PurchaseMonth,
        Trip
    }
}
