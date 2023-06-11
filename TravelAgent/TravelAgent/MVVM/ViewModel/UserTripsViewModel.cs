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
using TravelAgent.MVVM.ViewModel.Popup;

namespace TravelAgent.MVVM.ViewModel
{
    public class UserTripsViewModel : Core.ViewModel
    {
        public ObservableCollection<UserTripModel> UserTrips { get; set; }

        private Visibility _travelerVisibility;

        public Visibility TravelerVisibility
        {
            get { return _travelerVisibility; }
            set { _travelerVisibility = value; OnPropertyChanged(); }
        }

        private UserTripSearchPopup? _userTripsSearchPopup;
        private readonly UserTripSearchViewModel _userTripSearchViewModel;

        private readonly Service.UserTripService _userTripService;
        private readonly Service.NavigationService _navigationService;

        private bool _purchaseTripCommandRunning = false;
        private bool _cancelTripCommandRunning = false;

        public ICommand PurchaseTripCommand { get; }
        public ICommand OpenSearchCommand { get; }
        public ICommand CancelTripCommand { get; }

        public UserTripsViewModel(
            Service.UserTripService userTripService,
            Service.NavigationService navigationService,
            UserTripSearchViewModel userTripsSearchViewModel)
        {
            UserTrips = new ObservableCollection<UserTripModel>();
            TravelerVisibility = MainViewModel.SignedUser?.Type == UserType.Agent ? Visibility.Visible : Visibility.Collapsed;

            _userTripService = userTripService;
            _navigationService = navigationService;
            _userTripSearchViewModel = userTripsSearchViewModel;
            _userTripSearchViewModel.UserTripViewModel = this;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            PurchaseTripCommand = new RelayCommand(OnPurchaseTrip, o => !_purchaseTripCommandRunning);
            OpenSearchCommand = new RelayCommand(OnOpenSearch, o => true);
            CancelTripCommand = new RelayCommand(OnCancelTrip, o => !_cancelTripCommandRunning);

            if (MainViewModel.SignedUser?.Type == UserType.Traveler)
            {
                _ = LoadForUser();
            }
            else
            {
                _ = LoadAll();
            }
        }

        private void OnOpenSearch(object o)
        {
            _userTripsSearchPopup?.Close();
            _userTripsSearchPopup = new UserTripSearchPopup()
            {
                DataContext = _userTripSearchViewModel
            };
            _userTripsSearchPopup.Show();
        }

        private void OnNavigationCompleted(object? sender, Core.NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(UserTripsViewModel))
            {
                MainViewModel.RemoveOpenSearchKeyBinding();
                _userTripsSearchPopup?.Close();
                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }
            else
            {
                MainViewModel.AddOpenSearchKeyBinding(OpenSearchCommand);
            }
        }

        public async Task LoadAll()
        {
            UserTrips.Clear();
            IEnumerable<UserTripModel> userTrips = await _userTripService.GetAll();
            foreach (UserTripModel userTrip in userTrips)
            {
                UserTrips.Add(userTrip);
            }
        }

        public async Task LoadForUser()
        {
            UserTrips.Clear();
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
                _purchaseTripCommandRunning = true;

                int tripId = int.Parse(purchaseButton.Tag.ToString());
                UserTripModel userTrip = UserTrips.FirstOrDefault(userTrip => userTrip.Trip.Id == tripId);

                try
                {
                    await _userTripService.PurchaseReserved(userTrip.User.Id, userTrip.Trip.Id);
                    MessageBox.Show("Successful purchase!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    UserTrips.Clear();
                    IEnumerable<UserTripModel> userTrips = await _userTripService.GetForUser(MainViewModel.SignedUser.Id);
                    foreach (UserTripModel ut in userTrips)
                    {
                        UserTrips.Add(ut);
                    }
                }
                catch (DatabaseResponseException e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally 
                { 
                    _purchaseTripCommandRunning = false; 
                }
            }

        }

        private async void OnCancelTrip(object o)
        {
            if (o is Button cancelButton)
            {
                _cancelTripCommandRunning = true;

                int tripId = int.Parse(cancelButton.Tag.ToString());
                UserTripModel userTrip = UserTrips.FirstOrDefault(userTrip => userTrip.Trip.Id == tripId);

                try
                {
                    await _userTripService.Cancel(userTrip.User.Id, userTrip.Trip.Id);
                    MessageBox.Show("Trip successfully cancelled!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    UserTrips.Clear();
                    IEnumerable<UserTripModel> userTrips = await _userTripService.GetForUser(MainViewModel.SignedUser.Id);
                    foreach (UserTripModel ut in userTrips)
                    {
                        UserTrips.Add(ut);
                    }
                }
                catch (DatabaseResponseException e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    _cancelTripCommandRunning = false;
                }
            }
        }
    }
}
