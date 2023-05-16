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
        public ObservableCollection<TripModel> AllTrips { get; set; }
        public ObservableCollection<LocationModel> AllLocations { get; set; }

        private readonly TripService _tripService;
        private readonly LocationService _locationService;
        public Consts Consts { get; }

        public event EventHandler LoadFinished;

        public MapViewModel(
            Service.TripService tripService,
            LocationService locationService,
            Core.Consts consts)
        {
            _tripService = tripService;
            _locationService = locationService;
            Consts = consts;

            LoadAll();
        }

        private async void LoadAll()
        {
            await LoadTrips();
            await LoadLocations();

            LoadFinished?.Invoke(this, new EventArgs());
        }

        private async Task LoadTrips()
        {
            AllTrips = new ObservableCollection<TripModel>();

            IEnumerable<TripModel> allTrips = await _tripService.GetAll();
            foreach (TripModel trip in allTrips)
            {
                AllTrips.Add(trip);
            }
        }

        private async Task LoadLocations()
        {
            AllLocations = new ObservableCollection<LocationModel>();

            IEnumerable<LocationModel> allLocations = await _locationService.GetAll();
            foreach (LocationModel location in allLocations)
            {
                AllLocations.Add(location);
            }
        }
    }
}
