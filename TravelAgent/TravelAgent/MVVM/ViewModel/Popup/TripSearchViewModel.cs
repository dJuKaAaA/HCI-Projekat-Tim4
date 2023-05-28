using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel.Popup
{
    public class TripSearchViewModel : Core.ViewModel
    {
        private HashSet<TripSearchType> _searchTypes;

        private Model.TripSearchModel _tripSearchModel = new Model.TripSearchModel();

        public Model.TripSearchModel TripSearchModel
        {
            get { return _tripSearchModel; }
            set { _tripSearchModel = value; OnPropertyChanged(); }
        }

        private Visibility _departureVisibility = Visibility.Collapsed;

        public Visibility DepartureVisibility
        {
            get { return _departureVisibility; }
            set { _departureVisibility = value; OnPropertyChanged(); }
        }

        private bool _isDepartureChecked;

        public bool IsDepartureChecked
        {
            get { return _isDepartureChecked; }
            set 
            { 
                _isDepartureChecked = value; 
                OnPropertyChanged(); 
                if (_isDepartureChecked)
                {
                    _searchTypes.Add(TripSearchType.Departure);
                    DepartureVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(TripSearchType.Departure))
                    {
                        _searchTypes.Remove(TripSearchType.Departure);
                    }
                    DepartureVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _destinationVisibility = Visibility.Collapsed;

        public Visibility DestinationVisibility
        {
            get { return _destinationVisibility; }
            set { _destinationVisibility = value; OnPropertyChanged(); }
        }

        private bool _isDestinationChecked;

        public bool IsDestinationChecked
        {
            get { return _isDestinationChecked; }
            set 
            {
                _isDestinationChecked = value; 
                OnPropertyChanged(); 
                if (_isDestinationChecked)
                {
                    _searchTypes.Add(TripSearchType.Destination);
                    DestinationVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(TripSearchType.Destination))
                    {
                        _searchTypes.Remove(TripSearchType.Destination);
                    }
                    DestinationVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _departureDateVisibility = Visibility.Collapsed;

        public Visibility DepartureDateVisibility
        {
            get { return _departureDateVisibility; }
            set { _departureDateVisibility = value; OnPropertyChanged(); }
        }

        private bool _isDepartureDateChecked;

        public bool IsDepartureDateChecked
        {
            get { return _isDepartureDateChecked; }
            set 
            { 
                _isDepartureDateChecked = value; 
                OnPropertyChanged(); 
                if (_isDepartureDateChecked)
                {
                    _searchTypes.Add(TripSearchType.DepartureDateTime);
                    DepartureDateVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(TripSearchType.DepartureDateTime))
                    {
                        _searchTypes.Remove(TripSearchType.DepartureDateTime);
                    }
                    DepartureDateVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _arrivalDateVisibility = Visibility.Collapsed;

        public Visibility ArrivalDateVisibility
        {
            get { return _arrivalDateVisibility; }
            set { _arrivalDateVisibility = value; OnPropertyChanged(); }
        }

        private bool _isArrivalDateChecked;

        public bool IsArrivalDateChecked
        {
            get { return _isArrivalDateChecked; }
            set 
            { 
                _isArrivalDateChecked = value; 
                OnPropertyChanged();
                if (_isArrivalDateChecked)
                {
                    _searchTypes.Add(TripSearchType.ArrivalDateTime);
                    ArrivalDateVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(TripSearchType.ArrivalDateTime))
                    {
                        _searchTypes.Remove(TripSearchType.ArrivalDateTime);
                    }
                    ArrivalDateVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _priceVisibility = Visibility.Collapsed;

        public Visibility PriceVisibility
        {
            get { return _priceVisibility; }
            set { _priceVisibility = value; OnPropertyChanged(); }
        }
        
        private bool _isPriceChecked;

        public bool IsPriceChecked
        {
            get { return _isPriceChecked; }
            set 
            { 
                _isPriceChecked = value; 
                OnPropertyChanged(); 
                if (_isPriceChecked)
                {
                    _searchTypes.Add(TripSearchType.Price);
                    PriceVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(TripSearchType.Price))
                    {
                        _searchTypes.Remove(TripSearchType.Price);
                    }
                    PriceVisibility = Visibility.Collapsed;
                }
            }
        }

        private readonly TripService _tripService;

        public AllTripsViewModel AllTripsViewModel { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand CloseCommand { get; }

        public TripSearchViewModel(
            TripService tripService)
        {
            _searchTypes = new HashSet<TripSearchType>();

            _tripService = tripService;

            SearchCommand = new RelayCommand(OnSearch, CanSearch);
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private void SetValuesToDefault()
        {
            IsDepartureChecked = false;
            IsDestinationChecked = false;
            IsDepartureDateChecked = false;
            IsArrivalDateChecked = false;
            IsPriceChecked = false;
            _searchTypes.Clear();
            TripSearchModel = new Model.TripSearchModel();
        }

        private async void OnSearch(object o)
        {
            if (_searchTypes.Count > 0)
            {
                IEnumerable<Model.TripModel> trips = await _tripService.Search(_searchTypes, TripSearchModel);
                AllTripsViewModel.Trips.Clear();
                foreach (TripModel trip in trips)
                {
                    AllTripsViewModel.Trips.Add(trip);
                }
            }
            else
            {
                await AllTripsViewModel.LoadAll();
            }
            OnClose(this);
        }

        private bool CanSearch(object o)
        {
            bool canSearch = true;
            if (IsDepartureChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(TripSearchModel.DepartureSearchKeyword);
            }
            if (IsDestinationChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(TripSearchModel.DestinationSearchKeyword);
            }
            if (IsDepartureDateChecked)
            {
                canSearch = canSearch && (TripSearchModel.SelectedDepartureDate != null);
            }
            if (IsArrivalDateChecked)
            {
                canSearch = canSearch && (TripSearchModel.SelectedArrivalDate != null);
            }

            return canSearch;
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<TripSearchPopup>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();

            SetValuesToDefault();
        }
    }
}
