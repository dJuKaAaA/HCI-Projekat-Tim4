using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.Exception;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;
using TravelAgent.Service;

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

        public ObservableCollection<TouristAttractionModel> TouristAttractionsForTrip { get; set; }
        public ObservableCollection<RestaurantModel> RestaurantsForTrip { get; set; }
        public ObservableCollection<AccommodationModel> AccommodationsForTrip { get; set; }

        private int _tripDuration;

        public int TripDuration
        {
            get { return _tripDuration; }
            set { _tripDuration = value; OnPropertyChanged(); }
        }

        private Visibility _purchaseButtonsVisibility;

        public Visibility PurchaseButtonVisibility
        {
            get { return _purchaseButtonsVisibility; }
            set { _purchaseButtonsVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _cannotPurchaseTextVisibility;

        public Visibility CannotPurchaseTextVisibility
        {
            get { return _cannotPurchaseTextVisibility; }
            set { _cannotPurchaseTextVisibility = value; OnPropertyChanged(); }
        }

        public AllTripsViewModel AllTripsViewModel { get; set; }

        public Service.UserTripService UserTripService { get; set; }
        public Service.TouristAttractionService TouristAttractionService { get; set; }
        public Service.RestaurantService RestaurantService { get; set; }
        public Service.AccommodationService AccommodationsService { get; set; }
        public Service.MapService MapService { get; set; }
        public Consts Consts { get; set; }

        private bool _purchaseTripCommandRunning = false;
        private bool _reserveTripCommandRunning = false;

        public ICommand PurchaseTripCommand { get; }
        public ICommand ReserveTripCommand { get; }
        public ICommand CloseCommand { get; }

        public SeeDealViewModel(
            UserTripService userTripService,
            MapService mapService,
            TouristAttractionService touristAttractionService,
            RestaurantService restaurantService,
            AccommodationService accommodationService,
            Consts consts)
        {
            TouristAttractionsForTrip = new ObservableCollection<TouristAttractionModel>();
            RestaurantsForTrip = new ObservableCollection<RestaurantModel>();
            AccommodationsForTrip = new ObservableCollection<AccommodationModel>();

            PurchaseButtonVisibility = MainViewModel.SignedUser?.Type == UserType.Traveler ? Visibility.Visible : Visibility.Collapsed;
            CannotPurchaseTextVisibility = MainViewModel.SignedUser?.Type == UserType.Traveler ? Visibility.Collapsed : Visibility.Visible;

            UserTripService = userTripService;
            MapService = mapService;
            TouristAttractionService = touristAttractionService;
            RestaurantService = restaurantService;
            AccommodationsService = accommodationService;
            Consts = consts;

            PurchaseTripCommand = new RelayCommand(OnPurchaseTrip, o => !_purchaseTripCommandRunning);
            ReserveTripCommand = new RelayCommand(OnReserveTrip, o => !_reserveTripCommandRunning);
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        public async Task LoadTouristAttractionsForTrip()
        {
            TouristAttractionsForTrip.Clear();
            IEnumerable<TouristAttractionModel> touristAttractions = await TouristAttractionService.GetForTrip(Trip.Id);
            foreach (TouristAttractionModel touristAttraction in touristAttractions)
            {
                TouristAttractionsForTrip.Add(touristAttraction);
            }
        }

        public async Task LoadRestaurantsForTrip()
        {
            RestaurantsForTrip.Clear();
            IEnumerable<RestaurantModel> restaurants = await RestaurantService.GetForTrip(Trip.Id);
            foreach (RestaurantModel restauraunt in restaurants)
            {
                RestaurantsForTrip.Add(restauraunt);
            }
        }

        public async Task LoadAccommodationsForTrip()
        {
            AccommodationsForTrip.Clear();
            IEnumerable<AccommodationModel> accommodations = await AccommodationsService.GetForTrip(Trip.Id);
            foreach (AccommodationModel accommodation in accommodations)
            {
                AccommodationsForTrip.Add(accommodation);
            }
        }

        private async void OnPurchaseTrip(object o)
        {
            _purchaseTripCommandRunning = true;
            await CreateUserTrip(TripInvoiceType.Purchased);
            _purchaseTripCommandRunning = false;
        }

        private async void OnReserveTrip(object o)
        {
            _reserveTripCommandRunning = true;
            await CreateUserTrip(TripInvoiceType.Reserved);
            _reserveTripCommandRunning = false;
        }

        private async Task CreateUserTrip(TripInvoiceType type)
        {
            UserTripModel userTrip = new UserTripModel()
            {
                Trip = Trip,
                User = MainViewModel.SignedUser,
                Type = type,
                PurchaseDate = DateTime.Now,
            };
            try
            {
                await UserTripService.CreateNew(userTrip);
                string action = type == TripInvoiceType.Purchased ? "purchase" : "reservation";
                MessageBox.Show($"Successful {action}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                OnClose(this);
            }
            catch (DatabaseResponseException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<SeeDealPopup>().SingleOrDefault(w => w.IsActive);

            currentWindow?.Close();
        }
    }
}
