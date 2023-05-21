using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.Exception;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel.Popup
{
    public class SeeDealViewModel : Core.ViewModel
    {
        private TripModel _trip;

        public TripModel Trip
        {
            get { return _trip; }
            set { _trip = value; OnPropertyChanged(); }
        }

        private int _tripDuration;

        public int TripDuration
        {
            get { return _tripDuration; }
            set { _tripDuration = value; OnPropertyChanged(); }
        }

        public Service.UserTripService UserTripService { get; set; }

        public ICommand PurchaseTripCommand { get; }
        public ICommand ReserveTripCommand { get; }
        public ICommand CloseCommand { get; }

        public SeeDealViewModel()
        {
            PurchaseTripCommand = new RelayCommand(OnPurchaseTrip, o => true);
            ReserveTripCommand = new RelayCommand(OnReserveTrip, o => true);
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private async void OnPurchaseTrip(object o)
        {
            await CreateUserTrip(TripInvoiceType.Purchased);
        }

        private async void OnReserveTrip(object o)
        {
            await CreateUserTrip(TripInvoiceType.Reserved);
        }

        private async Task CreateUserTrip(TripInvoiceType type)
        {
            UserTripModel userTrip = new UserTripModel()
            {
                Trip = Trip,
                User = MainViewModel.SignedUser,
                Type = type
            };
            try
            {
                await UserTripService.CreateNew(userTrip);
                string action = type == TripInvoiceType.Purchased ? "purchase" : "reservation";
                MessageBox.Show($"Successful {action}!");
                OnClose(this);
            }
            catch (DatabaseResponseException e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<SeeDealPopup>().SingleOrDefault(w => w.IsActive);

            currentWindow?.Close();
        }
    }
}
