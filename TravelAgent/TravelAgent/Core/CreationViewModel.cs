using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Exception;
using TravelAgent.MVVM.Model;
using TravelAgent.Service;

namespace TravelAgent.Core
{
    public class CreationViewModel : Core.ViewModel
    {
        private LocationModel? _location;

        public LocationModel? Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged(); }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged(); }
        }

        public EventHandler<LocationModel> AddressSearched;

        public MapService MapService { get; }

        public ICommand SearchLocationFromAddressCommand { get; }
        public ICommand ClosePopupCommand { get; protected set; }

        public CreationViewModel(MapService mapService)
        {
            MapService = mapService;

            SearchLocationFromAddressCommand = new RelayCommand(OnSearchLocationFromAddress, o => !string.IsNullOrWhiteSpace(Address));
        }

        public async void OnSearchLocationFromAddress(object o)
        {
            try
            {
                Location = await MapService.Geocode(Address);
                AddressSearched?.Invoke(this, Location);
            }
            catch (LocationNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
