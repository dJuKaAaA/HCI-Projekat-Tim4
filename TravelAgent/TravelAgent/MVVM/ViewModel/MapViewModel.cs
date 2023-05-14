using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class MapViewModel : Core.ViewModel
    {
        public ObservableCollection<FlightModel> AllFlights { get; set; }

        private readonly FlightService _flightService;
        public Consts Consts { get; }

        public event EventHandler LoadedFlights;

        public MapViewModel(
            Service.FlightService flightService,
            Core.Consts consts)
        {
            _flightService = flightService;
            Consts = consts;
            
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

            LoadedFlights?.Invoke(this, new EventArgs());
        }
    }
}
