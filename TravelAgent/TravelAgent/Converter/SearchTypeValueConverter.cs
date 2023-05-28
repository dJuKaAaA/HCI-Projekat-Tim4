using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TravelAgent.Core;

namespace TravelAgent.Converter
{
    public class UserTripSearchTypeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserTripSearchType type)
            {
                switch (type)
                {
                    case UserTripSearchType.Departure:
                        return "Departure address";
                    case UserTripSearchType.Destination:
                        return "Destination address";
                    case UserTripSearchType.DepartureDateTime:
                        return "Departure date";
                    case UserTripSearchType.ArrivalDateTime:
                        return "Arrival date";
                    case UserTripSearchType.Price:
                        return "Price range";
                    case UserTripSearchType.PurchaseMonth:
                        return "Purchase month";
                    case UserTripSearchType.Trip:
                        return "Trip ID";
                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class TripSearchTypeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TripSearchType type)
            {
                switch (type)
                {
                    case TripSearchType.Departure:
                        return "Departure address";
                    case TripSearchType.Destination:
                        return "Destination address";
                    case TripSearchType.DepartureDateTime:
                        return "Departure date";
                    case TripSearchType.ArrivalDateTime:
                        return "Arrival date";
                    case TripSearchType.Price:
                        return "Price range";
                    default:
                        return string.Empty;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
