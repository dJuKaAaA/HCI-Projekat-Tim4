using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel
{
    public class CreateTripViewModel : Core.ViewModel
    {
        private TripModel? _tripForModification;

        private bool _modifying;

        public bool Modifying
        {
            get { return _modifying; }
            set { _modifying = value; OnPropertyChanged(); }
        }

        private CreateLocationPopup? _createLocationPopup;

        public ObservableCollection<LocationModel> Locations { get; set; }

        private LocationModel? _selectedDepartureLocation;

        public LocationModel? SelectedDepartureLocation
        {
            get { return _selectedDepartureLocation; }
            set { _selectedDepartureLocation = value; OnPropertyChanged(); }
        }

        private LocationModel? _selectedDestinationLocation;

        public LocationModel? SelectedDestinationLocation
        {
            get { return _selectedDestinationLocation; }
            set { _selectedDestinationLocation = value; OnPropertyChanged(); }
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

        private readonly Service.TripService _tripService;
        private readonly Service.LocationService _locationService;
        private readonly Service.NavigationService _navigationService;

        public ICommand CreateTripCommand { get; }
        public ICommand BackToAllTripsViewCommand { get; }
        public ICommand OpenCreateLocationPopupCommand { get; }

        public CreateTripViewModel(
            Service.TripService tripService,
            Service.LocationService locationService,
            Service.NavigationService navigationService)
        {
            Hours = new ObservableCollection<string>();
            Minutes = new ObservableCollection<string>();
            Locations = new ObservableCollection<LocationModel>();

            _tripService = tripService;
            _locationService = locationService;
            _navigationService = navigationService;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            CreateTripCommand = new RelayCommand(OnCreateTrip, CanCreateTrip);
            BackToAllTripsViewCommand = new RelayCommand(o => _navigationService.NavigateTo<AllTripsViewModel>(), o => true);
            OpenCreateLocationPopupCommand = new Core.RelayCommand(OnOpenCreateLocationPopup, o => true);

            SetUpForCreation();
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

            if (SelectedDepartureLocation?.Id == SelectedDestinationLocation?.Id)
            {
                MessageBox.Show("Departure and destination cannot be the same!");
                return;
            }


            TripModel trip = new TripModel()
            {
                Departure = SelectedDepartureLocation,
                Destination = SelectedDestinationLocation,
                DepartureDateTime = DepartureDateTime,
                ArrivalDateTime = ArrivalDateTime,
                Price = double.Parse(Price),
            };

            if (_tripForModification == null)
            {
                await _tripService.Create(trip);
                MessageBox.Show("Trip created successfully!");
                SetValuesToDefault();
            }
            else
            {
                await _tripService.Modify(_tripForModification.Id, trip);
                MessageBox.Show("Trip modified successfully!");
                _navigationService.NavigateTo<AllTripsViewModel>();
            }

        }

        private bool CanCreateTrip(object o)
        {
            return DepartureDate != null && ArrivalDate != null;
        }

        private void SetValuesToDefault()
        {
            SetDefaultSelectedLocations();
            DepartureDate = null;
            ArrivalDate = null;
            DepartureTimeHours = Hours[0];
            ArrivalTimeHours = Hours[0];
            DepartureTimeMinutes = Minutes[0];
            ArrivalTimeMinutes = Minutes[0];
            Price = "0";
        }
        
        private void SetValuesFromTrip()
        {
            SelectedDepartureLocation = Locations.FirstOrDefault(l => l.Id == _tripForModification.Departure.Id);
            SelectedDestinationLocation = Locations.FirstOrDefault(l => l.Id == _tripForModification.Destination.Id);
            DepartureDate = _tripForModification.DepartureDateTime;
            ArrivalDate = _tripForModification.ArrivalDateTime;
            DepartureTimeHours = Hours.FirstOrDefault(h => int.Parse(h) == _tripForModification.DepartureDateTime.Hour);
            DepartureTimeMinutes = Minutes.FirstOrDefault(m => int.Parse(m) == _tripForModification.DepartureDateTime.Minute);
            ArrivalTimeHours = Hours.FirstOrDefault(h => int.Parse(h) == _tripForModification.ArrivalDateTime.Hour);
            ArrivalTimeMinutes = Minutes.FirstOrDefault(m => int.Parse(m) == _tripForModification.ArrivalDateTime.Minute);
            Price = _tripForModification.Price.ToString();
        }

        private void OnNavigationCompleted(object? sender, Core.NavigationEventArgs e)
        {
            if (e.Extra is TripModel trip)
            {
                _tripForModification = trip;
                Modifying = true;
                SetUpForModification();
            }

            _createLocationPopup?.Close();
        }

        private async void SetUpForCreation()
        {
            SetUpTimeComboBoxes();
            await LoadLocations();
            SetValuesToDefault();
        }

        private async void SetUpForModification()
        {
            SetUpTimeComboBoxes();
            await LoadLocations();
            SetValuesFromTrip();
        }

        private async Task LoadLocations()
        {
            IEnumerable<LocationModel> locations = await _locationService.GetAll();
            foreach (LocationModel location in locations)
            {
                Locations.Add(location);
            }
            SetDefaultSelectedLocations();
        }

        private void SetDefaultSelectedLocations()
        {
            if (Locations.Count > 1)
            {
                SelectedDepartureLocation = Locations[0];
                SelectedDestinationLocation = Locations[1];
            }
            else if (Locations.Count == 1)
            {
                SelectedDepartureLocation = Locations[0];
                SelectedDestinationLocation = Locations[0];
            }
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

        private void OnOpenCreateLocationPopup(object o)
        {
            _createLocationPopup?.Close();
            _createLocationPopup = new CreateLocationPopup();
            _createLocationPopup.Show();
        }

    }
}
