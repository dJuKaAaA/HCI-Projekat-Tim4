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
using TravelAgent.MVVM.ViewModel.Popup;

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

        public ObservableCollection<TripModel> Trips { get; set; }

        private TripModel? _selectedTrip;

        public TripModel? SelectedTrip
        {
            get { return _selectedTrip; }
            set { _selectedTrip = value; OnPropertyChanged(); }
        }

        private SeeDealPopup? _seeDealPopup;
        private readonly SeeDealViewModel _seeDealViewModel;
        private TripSearchPopup? _tripSearchPopup;
        private readonly TripSearchViewModel _tripSearchViewModel;

        private bool _deleteTripCommandRunning = false;

        public ICommand OpenSeeDealPopupCommand { get; }
        public ICommand OpenCreateTripViewCommand { get; }
        public ICommand OpenModifyTripViewCommand { get; } // this is just the CreateTripView with a TripModel passed as parameter
        public ICommand DeleteTripCommand { get; }
        public ICommand OpenSearchCommand { get; }

        public AllTripsViewModel(
            Service.NavigationService navigationService, 
            Service.TripService tripService,
            TripSearchViewModel tripSearchViewModel,
            SeeDealViewModel seeDealViewModel)
        {
            SeeDealVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Traveler ?
                Visibility.Visible : Visibility.Collapsed;
            BusIconVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Traveler ?
                Visibility.Collapsed : Visibility.Visible;
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ?
                Visibility.Visible : Visibility.Collapsed;

            Trips = new ObservableCollection<TripModel>();

            _navigationService = navigationService;
            _tripService = tripService;
            _tripSearchViewModel = tripSearchViewModel;
            _tripSearchViewModel.AllTripsViewModel = this;
            _seeDealViewModel = seeDealViewModel;
            _seeDealViewModel.AllTripsViewModel = this;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenSeeDealPopupCommand = new Core.RelayCommand(OnOpenSeeDealPopup, o => true);
            OpenCreateTripViewCommand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateTripViewModel>(), o => MainViewModel.SignedUser?.Type == UserType.Agent);
            OpenModifyTripViewCommand = new Core.RelayCommand(o => _navigationService.NavigateTo<CreateTripViewModel>(SelectedTrip), o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedTrip != null);
            DeleteTripCommand = new Core.RelayCommand(OnDeleteTrip, o => MainViewModel.SignedUser?.Type == UserType.Agent && SelectedTrip != null && !_deleteTripCommandRunning);
            OpenSearchCommand = new RelayCommand(OnOpenSearch, o => true);

            _ = LoadAll();
        }

        private void OnOpenSearch(object o)
        {
            _tripSearchPopup?.Close();
            _tripSearchPopup = new TripSearchPopup()
            {
                DataContext = _tripSearchViewModel
            };
            _tripSearchPopup.Show();
        }

        private void OnNavigationCompleted(object? sender, Core.NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(AllTripsViewModel))
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.RemoveCUDKeyBindings();
                }
                MainViewModel.RemoveOpenSearchKeyBinding();

                _seeDealPopup?.Close();
                _tripSearchPopup?.Close();

                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }
            else
            {
                if (MainViewModel.SignedUser?.Type == UserType.Agent)
                {
                    MainViewModel.AddCUDKeyBindings(
                        OpenCreateTripViewCommand,
                        OpenModifyTripViewCommand,
                        DeleteTripCommand);
                }
                MainViewModel.AddOpenSearchKeyBinding(OpenSearchCommand);
            }

        }

        private void OnOpenSeeDealPopup(object o)
        {
            if (o is Button seeDealButton)
            {
                int tripId = int.Parse(seeDealButton.Tag.ToString());
                TripModel trip = Trips.FirstOrDefault(f => f.Id == tripId);

                _seeDealPopup?.Close();
                _seeDealViewModel.Trip = trip;
                DateTime takeoff = _seeDealViewModel.Trip.DepartureDateTime;
                DateTime landing = _seeDealViewModel.Trip.ArrivalDateTime;
                TimeSpan timeDiff = landing - takeoff;
                _seeDealViewModel.TripDuration = (int)timeDiff.TotalMilliseconds;
                _seeDealPopup = new SeeDealPopup()
                {
                    DataContext = _seeDealViewModel
                };
                _seeDealPopup.Show();
            }
        }

        public async Task LoadAll()
        {
            Trips.Clear();
            IEnumerable<TripModel> allTrip = await _tripService.GetAll();
            foreach (TripModel trip in allTrip)
            {
                Trips.Add(trip);
            }
        }

        private async void OnDeleteTrip(object o)
        {
            _deleteTripCommandRunning = true;
            
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this trip?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await _tripService.Delete(SelectedTrip.Id);
                await LoadAll();
                MessageBox.Show("Trip deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _deleteTripCommandRunning = false;
        }
    }
}
