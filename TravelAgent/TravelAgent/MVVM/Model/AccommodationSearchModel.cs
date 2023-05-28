using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;

namespace TravelAgent.MVVM.Model
{
    public class AccommodationSearchModel : ObservableObject
    {
        private string _nameKeyword;

        public string NameKeyword
        {
            get { return _nameKeyword; }
            set { _nameKeyword = value; OnPropertyChanged(); }
        }

        private string _addressKeyword;

        public string AddressKeyword
        {
            get { return _addressKeyword; }
            set { _addressKeyword = value; OnPropertyChanged(); }
        }

        private string _startRatingRange = "0";

        public string StartRatingRange
        {
            get { return _startRatingRange; }
            set
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _startRatingRange = "0";
                    }

                    double.Parse(value);

                    if (_startRatingRange == "0" && value.Length > 1)
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
                    _startRatingRange = value; 
                    OnPropertyChanged(); 
                }
                catch (FormatException) { }
            }
        }

        private string _endRatingRange = "0";

        public string EndRatingRange
        {
            get { return _endRatingRange; }
            set
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _endRatingRange = "0";
                    }

                    double.Parse(value);

                    if (_endRatingRange == "0" && value.Length > 1)
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
                    _endRatingRange = value; 
                    OnPropertyChanged(); 
                }
                catch (FormatException) { }
            }
        }

    }
}
