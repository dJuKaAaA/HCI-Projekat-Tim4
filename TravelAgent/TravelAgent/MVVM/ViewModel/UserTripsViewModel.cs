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
    public class UserTripsViewModel : Core.ViewModel
    {
        public ObservableCollection<UserTripModel> UserTrips { get; set; }

        private readonly Service.UserTripService _userTripService;

        public UserTripsViewModel(
            Service.UserTripService userTripService)
        {
            _userTripService = userTripService;

            LoadAll();
        }

        private async void LoadAll()
        {
            UserTrips = new ObservableCollection<UserTripModel>();
            IEnumerable<UserTripModel> userTrips = await _userTripService.GetForUser(MainViewModel.SignedUser.Id);
            foreach (UserTripModel userTrip in userTrips)
            {
                UserTrips.Add(userTrip);
            }
        }
    }
}
