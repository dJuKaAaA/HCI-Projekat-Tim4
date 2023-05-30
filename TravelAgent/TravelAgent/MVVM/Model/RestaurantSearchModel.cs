using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;

namespace TravelAgent.MVVM.Model
{
    public class RestaurantSearchModel : ObservableObject
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

        private int _stars = 1;

        public int Stars
        {
            get { return _stars; }
            set { _stars = value; OnPropertyChanged(); }
        }

    }
}
