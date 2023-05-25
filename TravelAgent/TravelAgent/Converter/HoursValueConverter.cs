using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TravelAgent.Converter
{
    // takes milliseconds and converts it into format "HH hours, mm minutes"
    public class HoursValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int millis = (int)value;
            int hours = millis / (1000 * 60 * 60);
            millis -= hours * 1000 * 60 * 60;
            int minutes = millis / (1000 * 60);
            if (minutes == 0)
            {
                return $"{hours}h";
            }
            return $"{hours}h and {minutes}min";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
