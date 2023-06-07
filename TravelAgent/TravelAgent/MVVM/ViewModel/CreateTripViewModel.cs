using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.Exception;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel
{
    public class CreateTripViewModel : Core.ViewModel
    {
        public TripModel? TripForModification { get; private set; }

        private bool _modifying;

        public bool Modifying
        {
            get { return _modifying; }
            set { _modifying = value; OnPropertyChanged(); }
        }

        private bool _changedDeparture;
        private bool _changedDestination;

        public ObservableCollection<AccommodationModel> AllAccommodations { get; set; }
        public ObservableCollection<RestaurantModel> AllRestaurants { get; set; }
        public ObservableCollection<TouristAttractionModel> AllTouristAttractions { get; set; }

        public ObservableCollection<AccommodationModel> AccommodationsForTrip { get; set; }
        public ObservableCollection<RestaurantModel> RestaurantsForTrip { get; set; }
        public ObservableCollection<TouristAttractionModel> TouristAttractionsForTrip { get; set; }

        private string _departureAddress;

        public string DepartureAddress
        {
            get { return _departureAddress; }
            set { _departureAddress = value; OnPropertyChanged(); }
        }

        private string _destinationAddress;

        public string DestinationAddress
        {
            get { return _destinationAddress; }
            set { _destinationAddress = value; OnPropertyChanged(); }
        }

        private LocationModel? _selectedDepartureLocation;

        public LocationModel? SelectedDepartureLocation
        {
            get { return _selectedDepartureLocation; }
            set 
            {
                _selectedDepartureLocation = value; 

                if (_selectedDepartureLocation != null)
                {
                    SelectedDepartureLabelVisibility = Visibility.Visible;
                    NADepartureVisibility = Visibility.Collapsed;
                }
                else
                {
                    SelectedDepartureLabelVisibility = Visibility.Collapsed;
                    NADepartureVisibility = Visibility.Visible;
                }

                OnPropertyChanged(); 
                _changedDeparture = true;
            }
        }

        private LocationModel? _selectedDestinationLocation;

        public LocationModel? SelectedDestinationLocation
        {
            get { return _selectedDestinationLocation; }
            set 
            {
                _selectedDestinationLocation = value; 

                if (_selectedDestinationLocation != null)
                {
                    SelectedDestinationLabelVisibility = Visibility.Visible;
                    NADestinationVisibility = Visibility.Collapsed;
                }
                else
                {
                    SelectedDestinationLabelVisibility= Visibility.Collapsed;
                    NADestinationVisibility = Visibility.Visible;
                }

                OnPropertyChanged(); 
                _changedDestination = true; 
            }
        }

        public ObservableCollection<string> Hours { get; set; }

        private string _departureTimeHours;

        public string DepartureTimeHours
        {
            get { return _departureTimeHours; }
            set { _departureTimeHours = value; OnPropertyChanged(); }
        }

        private string _arrivalTimeHours;

        public string ArrivalTimeHours
        {
            get { return _arrivalTimeHours; }
            set { _arrivalTimeHours = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Minutes { get; set; }

        private string _departureTimeMinutes;

        public string DepartureTimeMinutes
        {
            get { return _departureTimeMinutes; }
            set { _departureTimeMinutes = value; OnPropertyChanged(); }
        }

        private string _arrivalTimeMinutes;

        public string ArrivalTimeMinutes
        {
            get { return _arrivalTimeMinutes; }
            set { _arrivalTimeMinutes = value; OnPropertyChanged(); }
        }

        private DateTime? _departureDate;

        public DateTime? DepartureDate
        {
            get { return _departureDate; }
            set { _departureDate = value; OnPropertyChanged(); }
        }

        private DateTime? _arrivalDate;

        public DateTime? ArrivalDate
        {
            get { return _arrivalDate; }
            set { _arrivalDate = value; OnPropertyChanged(); }
        }

        private string _price = "0";

        public string Price
        {
            get { return _price; }
            set 
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _price = "0";
                    }

                    double doubleValue = double.Parse(value);
                    if (doubleValue > 1000000.0)
                    {
                        return;
                    }
                    if (value.Contains('-'))
                    {
                        return;
                    }

                    if (_price == "0" && value.Length > 1)
                    {
                        if (value[0] == '0')
                        {
                            value = value[1..];
                        }
                        else if (value[1] == '0')
                        {
                            value = value[0] + value[2..];
                        }
                    }
                    _price = value; 
                    OnPropertyChanged(); 
                }
                catch (FormatException) { }
            } 
        }

        private RestaurantModel? _selectedRestaurantFromAll;

        public RestaurantModel? SelectedRestaurantFromAll
        {
            get { return _selectedRestaurantFromAll; }
            set { _selectedRestaurantFromAll = value; OnPropertyChanged(); }
        }

        private AccommodationModel? _selectedAccommodationFromAll;

        public AccommodationModel? SelectedAccommodationFromAll
        {
            get { return _selectedAccommodationFromAll; }
            set { _selectedAccommodationFromAll = value; OnPropertyChanged(); }
        }

        private TouristAttractionModel? _selectedTouristAttractionFromAll;

        public TouristAttractionModel? SelectedTouristAttractionFromAll
        {
            get { return _selectedTouristAttractionFromAll; }
            set { _selectedTouristAttractionFromAll = value; OnPropertyChanged(); }
        }

        private RestaurantModel? _selectedRestaurantForTrip;

        public RestaurantModel? SelectedRestaurantForTrip
        {
            get { return _selectedRestaurantForTrip; }
            set { _selectedRestaurantForTrip = value; OnPropertyChanged(); }
        }

        private AccommodationModel? _selectedAccommodationForTrip;

        public AccommodationModel? SelectedAccommodationForTrip
        {
            get { return _selectedAccommodationForTrip; }
            set { _selectedAccommodationForTrip = value; OnPropertyChanged(); }
        }

        private TouristAttractionModel _selectedTouristAttractionForTrip;

        public TouristAttractionModel SelectedTouristAttractionForTrip
        {
            get { return _selectedTouristAttractionForTrip; }
            set { _selectedTouristAttractionForTrip = value; OnPropertyChanged(); }
        }

        private int _selectedTabIndex = 0;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set 
            {
                if (!CanNavigateFromLocationSelection())
                {
                    MessageBox.Show("You should select the departure and destination before proceeding!" +
                        "\nClick on the search button or double click the map to select a location", "Warning", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Warning);

                }
                _selectedTabIndex = value; 
                OnPropertyChanged(); 

                AreLocationsSelected = (SelectedDepartureLocation != null &&
                    SelectedDestinationLocation != null) || _selectedTabIndex == 0;
            }
        }

        private bool _areLocationsSelected = true;

        public bool AreLocationsSelected
        {
            get { return _areLocationsSelected; }
            set { _areLocationsSelected = value; OnPropertyChanged(); }
        }

        private Visibility _finalizationWarningVisibility = Visibility.Visible;

        public Visibility FinalizationWarningVisibility
        {
            get { return _finalizationWarningVisibility; }
            set { _finalizationWarningVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _naDepartureVisibility = Visibility.Visible;

        public Visibility NADepartureVisibility
        {
            get { return _naDepartureVisibility; }
            set { _naDepartureVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _naDestinationVisibility = Visibility.Visible;

        public Visibility NADestinationVisibility
        {
            get { return _naDestinationVisibility; }
            set { _naDestinationVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _selectedDepartureLabelVisibility = Visibility.Collapsed;

        public Visibility SelectedDepartureLabelVisibility
        {
            get { return _selectedDepartureLabelVisibility; }
            set { _selectedDepartureLabelVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _selectedDestinationLabelVisibility = Visibility.Collapsed;

        public Visibility SelectedDestinationLabelVisibility
        {
            get { return _selectedDestinationLabelVisibility; }
            set { _selectedDestinationLabelVisibility = value; OnPropertyChanged(); }
        }

        private readonly int _tabCount = 5;

        public EventHandler<LocationModel> DepartureAddressSearched;
        public EventHandler<LocationModel> DestinationAddressSearched;

        private readonly Service.TripService _tripService;
        private readonly Service.LocationService _locationService;
        private readonly Service.NavigationService _navigationService;
        private readonly Service.AccommodationService _accommodationService;
        private readonly Service.TouristAttractionService _touristAttractionService;
        private readonly Service.RestaurantService _restaurantService;
        public Service.MapService MapService { get; }
        public Consts Consts { get; }

        private bool _createTripCommandRunning = false;
        private bool _searchDepartureFromAddressCommandRunning = false;
        private bool _searchDestinationFromAddressCommandRunning = false;

        public ICommand ToNextTabCommand { get; }
        public ICommand ToPreviousTabCommand { get; }
        public ICommand TabControlPreviewKeyDownCommand { get; }
        public ICommand TabShiftControlPreviewKeyDownCommand { get; }
        public ICommand CreateTripCommand { get; }
        public ICommand SearchDepartureFromAddressCommand { get; }
        public ICommand SearchDestinationFromAddressCommand { get; }
        public ICommand AddRestaurantCommand { get; }
        public ICommand AddAccommodationCommand { get; }
        public ICommand AddTouristAttractionCommand { get; }
        public ICommand RemoveRestaurantCommand { get; }
        public ICommand RemoveAccommodationCommand { get; }
        public ICommand RemoveTouristAttractionCommand { get; }

        public CreateTripViewModel(
            Service.TripService tripService,
            Service.LocationService locationService,
            Service.NavigationService navigationService,
            Service.MapService mapService,
            Service.AccommodationService accommodationService,
            Service.TouristAttractionService touristAttractionService,
            Service.RestaurantService restaurantService,
            Consts consts)
        {
            AllAccommodations = new ObservableCollection<AccommodationModel>();
            AllRestaurants = new ObservableCollection<RestaurantModel>();
            AllTouristAttractions = new ObservableCollection<TouristAttractionModel>();

            AccommodationsForTrip = new ObservableCollection<AccommodationModel>();
            RestaurantsForTrip = new ObservableCollection<RestaurantModel>();
            TouristAttractionsForTrip = new ObservableCollection<TouristAttractionModel>();

            Hours = new ObservableCollection<string>();
            Minutes = new ObservableCollection<string>();

            _tripService = tripService;
            _locationService = locationService;
            _navigationService = navigationService;
            _accommodationService = accommodationService;
            _touristAttractionService = touristAttractionService;
            _restaurantService = restaurantService;
            MapService = mapService;
            Consts = consts;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            ToNextTabCommand = new RelayCommand(o => IncrementTabIndex(), o => true);
            ToPreviousTabCommand = new RelayCommand(o => DecrementTabIndex(), o => true);
            CreateTripCommand = new RelayCommand(OnCreateTrip, CanCreateTrip);
            SearchDepartureFromAddressCommand = new RelayCommand(OnSearchDepartureFromAddress, o => !string.IsNullOrWhiteSpace(DepartureAddress) && !_searchDepartureFromAddressCommandRunning);
            SearchDestinationFromAddressCommand = new RelayCommand(OnSearchDestinationFromAddress, o => !string.IsNullOrWhiteSpace(DestinationAddress) && !_searchDestinationFromAddressCommandRunning);
            AddRestaurantCommand = new RelayCommand(OnAddRestaurant, o => SelectedRestaurantFromAll != null);
            AddAccommodationCommand = new RelayCommand(OnAddAccommodation, o => SelectedAccommodationFromAll != null);
            AddTouristAttractionCommand = new RelayCommand(OnAddTouristAttraction, o => SelectedTouristAttractionFromAll != null);
            RemoveRestaurantCommand = new RelayCommand(OnRemoveRestaurant, o => SelectedRestaurantForTrip != null);
            RemoveAccommodationCommand = new RelayCommand(OnRemoveAccommodation, o => SelectedAccommodationForTrip != null);
            RemoveTouristAttractionCommand = new RelayCommand(OnRemoveTouristAttraction, o => SelectedTouristAttractionForTrip != null);

            SetUpForCreation();
        }

        private void IncrementTabIndex()
        {
            SelectedTabIndex = (SelectedTabIndex + 1) % _tabCount;
        }

        private void DecrementTabIndex()
        {
            if (SelectedTabIndex == 0)
            {
                SelectedTabIndex = _tabCount - 1;
            }
            else
            {
                --SelectedTabIndex;
            }
        }

        public bool CanNavigateFromLocationSelection()
        {
            if (SelectedDepartureLocation == null || SelectedDestinationLocation == null)
            {
                if (SelectedTabIndex == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnRemoveRestaurant(object o)
        {
            if (RestaurantsForTrip.Contains(SelectedRestaurantForTrip))
            {
                RestaurantsForTrip.Remove(SelectedRestaurantForTrip);
            }
        }
        private void OnRemoveAccommodation(object o)
        {
            if (AccommodationsForTrip.Contains(SelectedAccommodationForTrip))
            {
                AccommodationsForTrip.Remove(SelectedAccommodationForTrip);
            }
        }
        
        private void OnRemoveTouristAttraction(object o)
        {
            if (TouristAttractionsForTrip.Contains(SelectedTouristAttractionForTrip))
            {
                TouristAttractionsForTrip.Remove(SelectedTouristAttractionForTrip);
            }
        }

        private void OnAddRestaurant(object o)
        {
            if (!RestaurantsForTrip.Contains(SelectedRestaurantFromAll))
            {
                RestaurantsForTrip.Add(SelectedRestaurantFromAll);
            }
        }

        private void OnAddAccommodation(object o)
        {
            if (!AccommodationsForTrip.Contains(SelectedAccommodationFromAll))
            {
                AccommodationsForTrip.Add(SelectedAccommodationFromAll);
            }
        }

        private void OnAddTouristAttraction(object o)
        {
            if (!TouristAttractionsForTrip.Contains(SelectedTouristAttractionFromAll))
            {
                TouristAttractionsForTrip.Add(SelectedTouristAttractionFromAll);
            }

        }

        public async Task LoadAllRestaurants()
        {
            AllRestaurants.Clear();
            IEnumerable<RestaurantModel> restaurants = await _restaurantService.GetAll();
            foreach (RestaurantModel restaurant in restaurants)
            {
                AllRestaurants.Add(restaurant);
            }
        }

        public async Task LoadAllTouristAttractions()
        {
            AllTouristAttractions.Clear();
            IEnumerable<TouristAttractionModel> touristAttractions = await _touristAttractionService.GetAll();
            foreach (TouristAttractionModel touristAttraction in touristAttractions)
            {
                AllTouristAttractions.Add(touristAttraction);
            }
        }

        public async Task LoadAllAccommodations()
        {
            AllAccommodations.Clear();
            IEnumerable<AccommodationModel> accommodations = await _accommodationService.GetAll();
            foreach (AccommodationModel accommodation in accommodations)
            {
                AllAccommodations.Add(accommodation);
            }
        }

        public async Task LoadRestaurantsFromTrip()
        {
            RestaurantsForTrip.Clear();
            IEnumerable<RestaurantModel> restaurants = await _restaurantService.GetForTrip(TripForModification.Id);
            foreach (RestaurantModel restaurant in restaurants)
            {
                RestaurantsForTrip.Add(restaurant);
            }
        }

        public async Task LoadTouristAttractionsFromTrip()
        {
            TouristAttractionsForTrip.Clear();
            IEnumerable<TouristAttractionModel> touristAttractions = await _touristAttractionService.GetForTrip(TripForModification.Id);
            foreach (TouristAttractionModel touristAttraction in touristAttractions)
            {
                TouristAttractionsForTrip.Add(touristAttraction);
            }
        }

        public async Task LoadAccommodationsFromTrip()
        {
            AccommodationsForTrip.Clear();
            IEnumerable<AccommodationModel> accommodations = await _accommodationService.GetForTrip(TripForModification.Id);
            foreach (AccommodationModel accommodation in accommodations)
            {
                AccommodationsForTrip.Add(accommodation);
            }
        }

        private async void OnSearchDepartureFromAddress(object o)
        {
            _searchDepartureFromAddressCommandRunning = true;

            try
            {
                LocationModel location = await MapService.Geocode(DepartureAddress);
                DepartureAddressSearched?.Invoke(this, location);
            }
            catch (LocationNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _searchDepartureFromAddressCommandRunning = false;
            }
        }

        private async void OnSearchDestinationFromAddress(object o)
        {
            _searchDestinationFromAddressCommandRunning = true;

            try
            {
                LocationModel location = await MapService.Geocode(DestinationAddress);
                DestinationAddressSearched?.Invoke(this, location);
            }
            catch (LocationNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _searchDestinationFromAddressCommandRunning = false;
            }
        }

        private async void OnCreateTrip(object o)
        {
            _createTripCommandRunning = true;

            DateTime DepartureDateTime = new DateTime(DepartureDate.Value.Year, DepartureDate.Value.Month, DepartureDate.Value.Day,
                int.Parse(DepartureTimeHours), int.Parse(DepartureTimeMinutes), 0);
            DateTime ArrivalDateTime = new DateTime(ArrivalDate.Value.Year, ArrivalDate.Value.Month, ArrivalDate.Value.Day,
                int.Parse(ArrivalTimeHours), int.Parse(ArrivalTimeMinutes), 0);

            if (DepartureDateTime < DateTime.Now || ArrivalDateTime < DateTime.Now)
            {
                MessageBox.Show("Departure date or arrival date cannot be in the past!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _createTripCommandRunning = false;
                return;
            }
            if (DepartureDateTime >= ArrivalDateTime)
            {
                MessageBox.Show("Departure date must be before arrival date!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _createTripCommandRunning = false;
                return;
            }

            TripModel trip = new TripModel()
            {
                DepartureDateTime = DepartureDateTime,
                Departure = SelectedDepartureLocation,
                Destination = SelectedDestinationLocation,
                ArrivalDateTime = ArrivalDateTime,
                Price = double.Parse(Price),
            };

            if (!Modifying)
            {
                LocationModel departure = await _locationService.Create(SelectedDepartureLocation);
                LocationModel destination = await _locationService.Create(SelectedDestinationLocation);
                trip.Departure = departure;
                trip.Destination = destination;
                await _tripService.Create(
                    trip,
                    RestaurantsForTrip,
                    AccommodationsForTrip,
                    TouristAttractionsForTrip);
                MessageBox.Show("Trip created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                int departureForDeletionId = 0;
                if (_changedDeparture)
                {
                    departureForDeletionId = TripForModification.Departure.Id;
                    LocationModel departure = await _locationService.Create(SelectedDepartureLocation);
                    trip.Departure = departure;
                }

                int destinationForDeletionId = 0;
                if (_changedDestination)
                {
                    destinationForDeletionId = TripForModification.Destination.Id;
                    LocationModel destination = await _locationService.Create(SelectedDestinationLocation);
                    trip.Destination = destination;
                }

                await _tripService.Modify(
                    TripForModification.Id,
                    trip,
                    RestaurantsForTrip,
                    AccommodationsForTrip,
                    TouristAttractionsForTrip);
                if (departureForDeletionId != 0)
                {
                    await _locationService.Delete(departureForDeletionId);
                }
                if (destinationForDeletionId != 0)
                {
                    await _locationService.Delete(destinationForDeletionId);
                }

                MessageBox.Show("Trip modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _createTripCommandRunning = false;
            _navigationService.NavigateTo<AllTripsViewModel>();
        }

        private bool CanCreateTrip(object o)
        {
            bool canCreateTrip = DepartureDate != null 
                && ArrivalDate != null
                && SelectedDepartureLocation != null
                && SelectedDestinationLocation != null
                && Price != "0"
                && !_createTripCommandRunning;
            if (canCreateTrip)
            {
                FinalizationWarningVisibility = Visibility.Collapsed;
            }
            else
            {
                FinalizationWarningVisibility = Visibility.Visible;
            }
            return canCreateTrip;
        }

        private void SetValuesToDefault()
        {
            RestaurantsForTrip.Clear();
            AccommodationsForTrip.Clear();
            TouristAttractionsForTrip.Clear();
            DepartureDate = null;
            ArrivalDate = null;
            DepartureTimeHours = Hours[0];
            ArrivalTimeHours = Hours[0];
            DepartureTimeMinutes = Minutes[0];
            ArrivalTimeMinutes = Minutes[0];
            Price = "0";
        }
        
        private async void SetValuesFromTrip()
        {
            DepartureDate = TripForModification.DepartureDateTime;
            ArrivalDate = TripForModification.ArrivalDateTime;
            DepartureTimeHours = Hours.FirstOrDefault(h => int.Parse(h) == TripForModification.DepartureDateTime.Hour);
            DepartureTimeMinutes = Minutes.FirstOrDefault(m => int.Parse(m) == TripForModification.DepartureDateTime.Minute);
            ArrivalTimeHours = Hours.FirstOrDefault(h => int.Parse(h) == TripForModification.ArrivalDateTime.Hour);
            ArrivalTimeMinutes = Minutes.FirstOrDefault(m => int.Parse(m) == TripForModification.ArrivalDateTime.Minute);
            Price = TripForModification.Price.ToString();
            await LoadRestaurantsFromTrip();
            await LoadTouristAttractionsFromTrip();
            await LoadAccommodationsFromTrip();
        }

        private void OnNavigationCompleted(object? sender, Core.NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(CreateTripViewModel))
            {
                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            } 

            if (e.Extra is TripModel trip)
            {
                TripForModification = trip;
                Modifying = true;
                SetUpForModification();
            }
        }

        private async void SetUpForCreation()
        {
            SetUpTimeComboBoxes();
            SetValuesToDefault();
        }

        private async void SetUpForModification()
        {
            SetUpTimeComboBoxes();
            SetValuesFromTrip();
        }

        private void SetUpTimeComboBoxes()
        {
            for (int i = 0; i < 60; ++i)
            {
                if (i < 24)
                {
                    Hours.Add(i < 10 ? $"0{i}" : $"{i}");
                }
                Minutes.Add(i < 10 ? $"0{i}" : $"{i}");
            }
        }

    }
}
