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
        private readonly Service.MapService _mapService;
        private readonly Service.TouristAttractionService _touristAttractionService;
        private readonly Service.RestorauntService _restorauntService;
        private readonly Service.AccommodationService _accommodationService;
        private readonly Consts _consts;

        public ObservableCollection<TripModel> AllTrips { get; set; }

        private TripModel? _selectedTrip;

        public TripModel? SelectedTrip
        {
            get { return _selectedTrip; }
            set { _selectedTrip = value; OnPropertyChanged(); }
        }

        private SeeDealPopup? _seeDealPopup;

        private KeyBinding? _openCreateTripViewKeyBinding;
        private KeyBinding? _openModifyTripViewKeyBinding;
        private KeyBinding? _deleteTripKeyBinding;

        public ICommand OpenSeeDealPopupCommand { get; }
        public ICommand OpenCreateTripViewCommand { get; }
        public ICommand OpenModifyTripViewCommand { get; } // this is just the CreateTripView with a TripModel passed as parameter
        public ICommand DeleteTripCommand { get; }

        public AllTripsViewModel(
            Service.NavigationService navigationService, 
            Service.TripService tripService,
            Service.UserTripService userTripService,
            Service.MapService mapService,
            Service.TouristAttractionService touristAttractionService,
            Service.RestorauntService restorauntService,
            Service.AccommodationService accommodationService,
            Consts consts)
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
            _mapService = mapService;
            _touristAttractionService = touristAttractionService;
            _restorauntService = restorauntService;
            _accommodationService = accommodationService;
            _consts = consts;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenSeeDealPopupCommand = new Core.RelayCommand(OnOpenSeeDealPopup, o => true);
            OpenCreateTripViewCommand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateTripViewModel>(), o => MainViewModel.SignedUser?.Type == UserType.Agent);
            OpenModifyTripViewCommand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateTripViewModel>(SelectedTrip), o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedTrip != null);
            DeleteTripCommand = new Core.RelayCommand(OnDeleteTrip, o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedTrip != null);

            LoadAll();
        }

        private void OnNavigationCompleted(object? sender, Core.NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(AllTripsViewModel))
            {
                Window window = Application.Current.MainWindow;
                window.InputBindings.Remove(_openCreateTripViewKeyBinding);
                window.InputBindings.Remove(_openModifyTripViewKeyBinding);
                window.InputBindings.Remove(_deleteTripKeyBinding);

                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }
            else
            {
                Window window = Application.Current.MainWindow;
                _openCreateTripViewKeyBinding = new KeyBinding(OpenCreateTripViewCommand, Key.N, ModifierKeys.Control);
                _openModifyTripViewKeyBinding = new KeyBinding(OpenModifyTripViewCommand, Key.C, ModifierKeys.Control);
                _deleteTripKeyBinding = new KeyBinding(DeleteTripCommand, Key.D, ModifierKeys.Control);
                window.InputBindings.Add(_openCreateTripViewKeyBinding);
                window.InputBindings.Add(_openModifyTripViewKeyBinding);
                window.InputBindings.Add(_deleteTripKeyBinding);
            }

            _seeDealPopup?.Close();
        }

        private void OnOpenSeeDealPopup(object o)
        {
            if (o is Button seeDealButton)
            {
                int tripId = int.Parse(seeDealButton.Tag.ToString());
                TripModel trip = AllTrips.FirstOrDefault(f => f.Id == tripId);

                _seeDealPopup?.Close();
                _seeDealPopup = new SeeDealPopup(
                    trip, 
                    _userTripService, 
                    _mapService,
                    _touristAttractionService,
                    _restorauntService,
                    _accommodationService,
                    _consts);
                _seeDealPopup.Show();
            }
        }

        private async void LoadAll()
        {
            AllTrips.Clear();
            IEnumerable<TripModel> allTrip = await _tripService.GetAll();
            foreach (TripModel trip in allTrip)
            {
                AllTrips.Add(trip);
            }
        }

        private async void OnDeleteTrip(object o)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this trip?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _tripService.Delete(SelectedTrip.Id);
                LoadAll();
                MessageBox.Show("Trip deleted successfully!");
            }
        }
    }
}
