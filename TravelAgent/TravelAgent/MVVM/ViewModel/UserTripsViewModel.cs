using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.Exception;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel
{
    public class UserTripsViewModel : Core.ViewModel
    {
        public ObservableCollection<UserTripModel> UserTrips { get; set; }

        private readonly Service.UserTripService _userTripService;

        public ICommand PurchaseTripCommand { get; }
        public ICommand CancelTripCommand { get; }

        public UserTripsViewModel(
            Service.UserTripService userTripService)
        {
            _userTripService = userTripService;

            PurchaseTripCommand = new RelayCommand(OnPurchaseTrip, o => true);
            CancelTripCommand = new RelayCommand(OnCancelTrip, o => true);

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

        private async void OnPurchaseTrip(object o)
        {
            if (o is Button purchaseButton)
            {
                int tripId = int.Parse(purchaseButton.Tag.ToString());
                UserTripModel userTrip = UserTrips.FirstOrDefault(userTrip => userTrip.Trip.Id == tripId);

                try
                {
                    await _userTripService.PurchaseReserved(userTrip.User.Id, userTrip.Trip.Id);
                    MessageBox.Show("Successful purchase!");

                    UserTrips.Clear();
                    IEnumerable<UserTripModel> userTrips = await _userTripService.GetForUser(MainViewModel.SignedUser.Id);
                    foreach (UserTripModel ut in userTrips)
                    {
                        UserTrips.Add(ut);
                    }
                }
                catch (DatabaseResponseException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private async void OnCancelTrip(object o)
        {
            if (o is Button cancelButton)
            {
                int tripId = int.Parse(cancelButton.Tag.ToString());
                UserTripModel userTrip = UserTrips.FirstOrDefault(userTrip => userTrip.Trip.Id == tripId);

                try
                {
                    await _userTripService.Cancel(userTrip.User.Id, userTrip.Trip.Id);
                    MessageBox.Show("Trip cancelled!");

                    UserTrips.Clear();
                    IEnumerable<UserTripModel> userTrips = await _userTripService.GetForUser(MainViewModel.SignedUser.Id);
                    foreach (UserTripModel ut in userTrips)
                    {
                        UserTrips.Add(ut);
                    }
                }
                catch (DatabaseResponseException e)
                {
                    MessageBox.Show(e.Message);
                }

            }

        }
    }
}
