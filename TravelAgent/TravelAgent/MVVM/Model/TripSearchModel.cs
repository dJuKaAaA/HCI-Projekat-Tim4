using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;

namespace TravelAgent.MVVM.Model
{
    public class TripSearchModel : ObservableObject
    {
        private string _departureSearchKeyword;

        public string DepartureSearchKeyword
        {
            get { return _departureSearchKeyword; }
            set { _departureSearchKeyword = value; OnPropertyChanged(); }
        }

        private string _destinationSearchKeyword;

        public string DestinationSearchKeyword
        {
            get { return _destinationSearchKeyword; }
            set { _destinationSearchKeyword = value; OnPropertyChanged(); }
        }

        private DateTime? _selectedDepartureDate;

        public DateTime? SelectedDepartureDate
        {
            get { return _selectedDepartureDate; }
            set { _selectedDepartureDate = value; OnPropertyChanged(); }
        }

        private DateTime? _selectedArrivalDate;

        public DateTime? SelectedArrivalDate
        {
            get { return _selectedArrivalDate; }
            set { _selectedArrivalDate = value; OnPropertyChanged(); }
        }

        private string _startPriceRange = "0";

        public string StartPriceRange
        {
            get { return _startPriceRange; }
            set
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _startPriceRange = "0";
                    }

                    double.Parse(value);

                    if (_startPriceRange == "0" && value.Length > 1)
                    {
                        if (value[0] == '0')
                        {
                            value = value[1..];
                        }
                        else if (value[1] == '0')
                        {
                            value = value[0] + value[2..];
                        }
                    }
                    _startPriceRange = value; 
                    OnPropertyChanged(); 
                }
                catch (FormatException) { }
            }
        }

        private string _endPriceRange = "0";

        public string EndPriceRange
        {
            get { return _endPriceRange; }
            set
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _endPriceRange = "0";
                    }

                    double.Parse(value);

                    if (_endPriceRange == "0" && value.Length > 1)
                    {
                        if (value[0] == '0')
                        {
                            value = value[1..];
                        }
                        else if (value[1] == '0')
                        {
                            value = value[0] + value[2..];
                        }
                    }
                    _endPriceRange = value; 
                    OnPropertyChanged(); 
                }
                catch (FormatException) { }
            }
        }

    }
}
