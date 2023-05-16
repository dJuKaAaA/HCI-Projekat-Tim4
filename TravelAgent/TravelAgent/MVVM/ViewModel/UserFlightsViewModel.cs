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
    public class UserFlightsViewModel : Core.ViewModel
    {
        public ObservableCollection<UserFlightModel> UserFlights { get; set; }

        private readonly Service.UserFlightService _userFlightService;

        public UserFlightsViewModel(
            Service.UserFlightService userFlightService)
        {
            _userFlightService = userFlightService;

            LoadAll();
        }

        private async void LoadAll()
        {
            UserFlights = new ObservableCollection<UserFlightModel>();
            IEnumerable<UserFlightModel> userFlights = await _userFlightService.GetForUser(MainViewModel.SignedUser.Id);
            foreach (UserFlightModel userFlight in userFlights)
            {
                UserFlights.Add(userFlight);
            }
        }
    }
}
