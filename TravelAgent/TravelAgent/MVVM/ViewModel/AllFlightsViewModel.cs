using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TravelAgent.MVVM.Model;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllFlightsViewModel : Core.ViewModel
    {
        private readonly Service.NavigationService _navigationService;
        private readonly Service.FlightService _flightService;

        public ObservableCollection<FlightModel> AllFlights { get; set; }

        public ICommand LogoutCommand { get; }

        public AllFlightsViewModel(
            Service.NavigationService navigationService, 
            Service.FlightService flightService)
        {
            _navigationService = navigationService;
            _flightService = flightService;

            LogoutCommand = new Core.RelayCommand((object o) => _navigationService.NavigateTo<LoginViewModel>(), (object o) => true);

            LoadAll();
        }

        private async void LoadAll()
        {
            AllFlights = new ObservableCollection<FlightModel>();

            IEnumerable<FlightModel> allFlights = await _flightService.GetAll();
            foreach (FlightModel flight in allFlights)
            {
                AllFlights.Add(flight);
            }
        }

    }
}
