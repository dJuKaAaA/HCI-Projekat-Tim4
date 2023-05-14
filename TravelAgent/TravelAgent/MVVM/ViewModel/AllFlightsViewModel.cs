using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllFlightsViewModel : Core.ViewModel
    {
        private readonly Service.NavigationService _navigationService;
        private readonly Service.FlightService _flightService;

        public ObservableCollection<FlightModel> AllFlights { get; set; }

        public ICommand OpenMapLocationDetailsViewCommand { get; }
        public ICommand OpenSeeDealPopupCommand { get; }

        public AllFlightsViewModel(
            Service.NavigationService navigationService, 
            Service.FlightService flightService)
        {
            _navigationService = navigationService;
            _flightService = flightService;

            OpenMapLocationDetailsViewCommand = new Core.RelayCommand(o => _navigationService.NavigateTo<MapLocationDetailsViewModel>(), o => true);
            OpenSeeDealPopupCommand = new Core.RelayCommand(OnOpenSeeDealPopup , o => true);

            LoadAll();
        }

        private void OnOpenSeeDealPopup(object o)
        {
            if (o is Button seeDealButton)
            {
                double flightId = double.Parse(seeDealButton.Tag.ToString());
                FlightModel flight = AllFlights.FirstOrDefault(f => f.Id == flightId);
                SeeDealPopup popup = new SeeDealPopup(flight);
                popup.Show();
            }
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
