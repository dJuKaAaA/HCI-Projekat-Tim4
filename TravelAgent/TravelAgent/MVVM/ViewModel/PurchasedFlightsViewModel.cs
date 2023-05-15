using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel
{
    public class PurchasedFlightsViewModel : Core.ViewModel
    {
        public ObservableCollection<FlightModel> AllFlights { get; set; }

        private readonly Service.FlightService _flightService;

        public PurchasedFlightsViewModel(
            Service.FlightService flightService)
        {
            _flightService = flightService;

            LoadAll();
        }

        private async void LoadAll()
        {
            AllFlights = new ObservableCollection<FlightModel>();
            IEnumerable<FlightModel> flights = await _flightService.GetAll();
            foreach (FlightModel flight in flights)
            {
                AllFlights.Add(flight);
            }
        }
    }
}
