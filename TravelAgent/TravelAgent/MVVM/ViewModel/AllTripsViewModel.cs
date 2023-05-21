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
    public class AllTripsViewModel : Core.ViewModel
    {
        private Visibility _seeDealVisibility;

        public Visibility SeeDealVisibility
        {
            get { return _seeDealVisibility; }
            set { _seeDealVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _busIconVisibility ;

        public Visibility BusIconVisibility
        {
            get { return _busIconVisibility; }
            set { _busIconVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        private readonly Service.NavigationService _navigationService;
        private readonly Service.TripService _tripService;
        private readonly Service.UserTripService _userTripService;

        public ObservableCollection<TripModel> AllTrips { get; set; }

        private TripModel? _selectedTrip;

        public TripModel? SelectedTrip
        {
            get { return _selectedTrip; }
            set { _selectedTrip = value; OnPropertyChanged(); }
        }

        private SeeDealPopup? _seeDealPopup;

        public ICommand OpenMapLocationDetailsViewCommand { get; }
        public ICommand OpenSeeDealPopupCommand { get; }
        public ICommand OpenCreateTripViewCommand { get; }

        public AllTripsViewModel(
            Service.NavigationService navigationService, 
            Service.TripService tripService,
            Service.UserTripService userTripService)
        {
            SeeDealVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Traveler ?
                Visibility.Visible : Visibility.Collapsed;
            BusIconVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Traveler ? 
                Visibility.Collapsed : Visibility.Visible;
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? 
                Visibility.Visible : Visibility.Collapsed;

            AllTrips = new ObservableCollection<TripModel>();

            _navigationService = navigationService;
            _tripService = tripService;
            _userTripService = userTripService;

            OpenMapLocationDetailsViewCommand = new Core.RelayCommand(o => _navigationService.NavigateTo<MapLocationDetailsViewModel>(), o => true);
            OpenSeeDealPopupCommand = new Core.RelayCommand(OnOpenSeeDealPopup , o => true);
            OpenCreateTripViewCommand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateTripViewModel>(), o => true);

            LoadAll();
        }

        private void OnOpenSeeDealPopup(object o)
        {
            if (o is Button seeDealButton)
            {
                int tripId = int.Parse(seeDealButton.Tag.ToString());
                TripModel trip = AllTrips.FirstOrDefault(f => f.Id == tripId);

                _seeDealPopup?.Close();
                _seeDealPopup = new SeeDealPopup(trip, _userTripService);
                _seeDealPopup.Show();
            }
        }

        private async void LoadAll()
        {
            AllTrips = new ObservableCollection<TripModel>();

            IEnumerable<TripModel> allTrip = await _tripService.GetAll();
            foreach (TripModel trip in allTrip)
            {
                AllTrips.Add(trip);
            }
        }

    }
}
