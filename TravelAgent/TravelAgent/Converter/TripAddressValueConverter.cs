using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TravelAgent.Converter
{
    // used for text wrapping when the TextWrapping attribute doesn't work
    public class TripAddressValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string address = (string)value;
            string wrappedAddress = "";
            bool wrap = false;
            for (int i = 0; i < address.Length; ++i)
            {
                if (i > 0 && i % 25 == 0)
                {
                    wrap = true;
                }
                if (address[i] == ' ' && wrap)
                {
                    wrappedAddress += "\n";
                    wrap = false;
                }
                else
                {
                    wrappedAddress += address[i];
                }
            }
            return wrappedAddress;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
