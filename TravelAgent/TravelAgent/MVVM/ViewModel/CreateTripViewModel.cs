using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public ObservableCollection<RestorauntModel> AllRestoraunts { get; set; }
        public ObservableCollection<TouristAttractionModel> AllTouristAttractions { get; set; }

        public ObservableCollection<AccommodationModel> AccommodationsForTrip { get; set; }
        public ObservableCollection<RestorauntModel> RestorauntsForTrip { get; set; }
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
            set { _selectedDepartureLocation = value; OnPropertyChanged(); _changedDeparture = true; }
        }

        private LocationModel? _selectedDestinationLocation;

        public LocationModel? SelectedDestinationLocation
        {
            get { return _selectedDestinationLocation; }
            set { _selectedDestinationLocation = value; OnPropertyChanged(); _changedDestination = true; }
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

                    double.Parse(value);

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

        private RestorauntModel? _selectedRestorauntFromAll;

        public RestorauntModel? SelectedRestorauntFromAll
        {
            get { return _selectedRestorauntFromAll; }
            set { _selectedRestorauntFromAll = value; OnPropertyChanged(); }
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

        private RestorauntModel? _selectedRestorauntForTrip;

        public RestorauntModel? SelectedRestorauntForTrip
        {
            get { return _selectedRestorauntForTrip; }
            set { _selectedRestorauntForTrip = value; OnPropertyChanged(); }
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

        public EventHandler<LocationModel> DepartureAddressSearched;
        public EventHandler<LocationModel> DestinationAddressSearched;

        private readonly Service.TripService _tripService;
        private readonly Service.LocationService _locationService;
        private readonly Service.NavigationService _navigationService;
        private readonly Service.AccommodationService _accommodationService;
        private readonly Service.TouristAttractionService _touristAttractionService;
        private readonly Service.RestorauntService _restorauntService;
        public Service.MapService MapService { get; }
        public Consts Consts { get; }

        public ICommand CreateTripCommand { get; }
        public ICommand SearchDepartureFromAddressCommand { get; }
        public ICommand SearchDestinationFromAddressCommand { get; }
        public ICommand AddRestorauntCommand { get; }
        public ICommand AddAccommodationCommand { get; }
        public ICommand AddTouristAttractionCommand { get; }
        public ICommand RemoveRestorauntCommand { get; }
        public ICommand RemoveAccommodationCommand { get; }
        public ICommand RemoveTouristAttractionCommand { get; }

        public CreateTripViewModel(
            Service.TripService tripService,
            Service.LocationService locationService,
            Service.NavigationService navigationService,
            Service.MapService mapService,
            Service.AccommodationService accommodationService,
            Service.TouristAttractionService touristAttractionService,
            Service.RestorauntService restorauntService,
            Consts consts)
        {
            AllAccommodations = new ObservableCollection<AccommodationModel>();
            AllRestoraunts = new ObservableCollection<RestorauntModel>();
            AllTouristAttractions = new ObservableCollection<TouristAttractionModel>();

            AccommodationsForTrip = new ObservableCollection<AccommodationModel>();
            RestorauntsForTrip = new ObservableCollection<RestorauntModel>();
            TouristAttractionsForTrip = new ObservableCollection<TouristAttractionModel>();

            Hours = new ObservableCollection<string>();
            Minutes = new ObservableCollection<string>();

            _tripService = tripService;
            _locationService = locationService;
            _navigationService = navigationService;
            _accommodationService = accommodationService;
            _touristAttractionService = touristAttractionService;
            _restorauntService = restorauntService;
            MapService = mapService;
            Consts = consts;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            CreateTripCommand = new RelayCommand(OnCreateTrip, CanCreateTrip);
            SearchDepartureFromAddressCommand = new RelayCommand(OnSearchDepartureFromAddress, o => !string.IsNullOrWhiteSpace(DepartureAddress));
            SearchDestinationFromAddressCommand = new RelayCommand(OnSearchDestinationFromAddress, o => !string.IsNullOrWhiteSpace(DestinationAddress));
            AddRestorauntCommand = new RelayCommand(OnAddRestoraunt, o => SelectedRestorauntFromAll != null);
            AddAccommodationCommand = new RelayCommand(OnAddAccommodation, o => SelectedAccommodationFromAll != null);
            AddTouristAttractionCommand = new RelayCommand(OnAddTouristAttraction, o => SelectedTouristAttractionFromAll != null);
            RemoveRestorauntCommand = new RelayCommand(OnRemoveRestoraunt, o => SelectedRestorauntForTrip != null);
            RemoveAccommodationCommand = new RelayCommand(OnRemoveAccommodation, o => SelectedAccommodationForTrip != null);
            RemoveTouristAttractionCommand = new RelayCommand(OnRemoveTouristAttraction, o => SelectedTouristAttractionForTrip != null);

            SetUpForCreation();
        }

        private void OnRemoveRestoraunt(object o)
        {
            if (RestorauntsForTrip.Contains(SelectedRestorauntForTrip))
            {
                RestorauntsForTrip.Remove(SelectedRestorauntForTrip);
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

        private void OnAddRestoraunt(object o)
        {
            if (!RestorauntsForTrip.Contains(SelectedRestorauntFromAll))
            {
                RestorauntsForTrip.Add(SelectedRestorauntFromAll);
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

        public async Task LoadAllRestoraunts()
        {
            AllRestoraunts.Clear();
            IEnumerable<RestorauntModel> restoraunts = await _restorauntService.GetAll();
            foreach (RestorauntModel restoraunt in restoraunts)
            {
                AllRestoraunts.Add(restoraunt);
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

        public async Task LoadRestorauntsFromTrip()
        {
            RestorauntsForTrip.Clear();
            IEnumerable<RestorauntModel> restoraunts = await _restorauntService.GetForTrip(TripForModification.Id);
            foreach (RestorauntModel restoraunt in restoraunts)
            {
                RestorauntsForTrip.Add(restoraunt);
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
            try
            {
                LocationModel location = await MapService.Geocode(DepartureAddress);
                DepartureAddressSearched?.Invoke(this, location);
            }
            catch (LocationNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnSearchDestinationFromAddress(object o)
        {
            try
            {
                LocationModel location = await MapService.Geocode(DestinationAddress);
                DestinationAddressSearched?.Invoke(this, location);
            }
            catch (LocationNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnCreateTrip(object o)
        {
            DateTime DepartureDateTime = new DateTime(DepartureDate.Value.Year, DepartureDate.Value.Month, DepartureDate.Value.Day,
                int.Parse(DepartureTimeHours), int.Parse(DepartureTimeMinutes), 0);
            DateTime ArrivalDateTime = new DateTime(ArrivalDate.Value.Year, ArrivalDate.Value.Month, ArrivalDate.Value.Day,
                int.Parse(ArrivalTimeHours), int.Parse(ArrivalTimeMinutes), 0);

            if (DepartureDateTime < DateTime.Now || ArrivalDateTime < DateTime.Now)
            {
                MessageBox.Show("Departure date or arrival date cannot be in the past!");
                return;
            }
            if (DepartureDateTime >= ArrivalDateTime)
            {
                MessageBox.Show("Departure date must be before arrival date!");
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

            if (TripForModification == null)
            {
                LocationModel departure = await _locationService.Create(SelectedDepartureLocation);
                LocationModel destination = await _locationService.Create(SelectedDestinationLocation);
                trip.Departure = departure;
                trip.Destination = destination;
                await _tripService.Create(
                    trip,
                    RestorauntsForTrip,
                    AccommodationsForTrip,
                    TouristAttractionsForTrip);
                MessageBox.Show("Trip created successfully!");
                SetValuesToDefault();
            }
            else
            {
                if (_changedDeparture)
                {
                    LocationModel departure = await _locationService.Create(SelectedDepartureLocation);
                    trip.Departure = departure;
                }
                if (_changedDestination)
                {
                    LocationModel destination = await _locationService.Create(SelectedDestinationLocation);
                    trip.Destination = destination;
                }
                await _tripService.Modify(
                    TripForModification.Id,
                    trip,
                    RestorauntsForTrip,
                    AccommodationsForTrip,
                    TouristAttractionsForTrip);
                MessageBox.Show("Trip modified successfully!");
                _navigationService.NavigateTo<AllTripsViewModel>();
            }

        }

        private bool CanCreateTrip(object o)
        {
            return DepartureDate != null 
                && ArrivalDate != null
                && SelectedDepartureLocation != null
                && SelectedDestinationLocation != null;
        }

        private void SetValuesToDefault()
        {
            RestorauntsForTrip.Clear();
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
            await LoadRestorauntsFromTrip();
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
