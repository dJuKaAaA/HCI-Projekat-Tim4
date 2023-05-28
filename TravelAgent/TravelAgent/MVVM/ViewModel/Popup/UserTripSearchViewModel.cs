using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel.Popup
{
    public class UserTripSearchViewModel : Core.ViewModel
    {
        private HashSet<UserTripSearchType> _searchTypes;

        private Model.UserTripSearchModel _userTripSearchModel = new Model.UserTripSearchModel();

        public Model.UserTripSearchModel UserTripSearchModel
        {
            get { return _userTripSearchModel; }
            set { _userTripSearchModel = value; OnPropertyChanged(); }
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
                    _searchTypes.Add(UserTripSearchType.Departure);
                    DepartureVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(UserTripSearchType.Departure))
                    {
                        _searchTypes.Remove(UserTripSearchType.Departure);
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
                    _searchTypes.Add(UserTripSearchType.Destination);
                    DestinationVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(UserTripSearchType.Destination))
                    {
                        _searchTypes.Remove(UserTripSearchType.Destination);
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
                    _searchTypes.Add(UserTripSearchType.DepartureDateTime);
                    DepartureDateVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(UserTripSearchType.DepartureDateTime))
                    {
                        _searchTypes.Remove(UserTripSearchType.DepartureDateTime);
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
                    _searchTypes.Add(UserTripSearchType.ArrivalDateTime);
                    ArrivalDateVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(UserTripSearchType.ArrivalDateTime))
                    {
                        _searchTypes.Remove(UserTripSearchType.ArrivalDateTime);
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
                    _searchTypes.Add(UserTripSearchType.Price);
                    PriceVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(UserTripSearchType.Price))
                    {
                        _searchTypes.Remove(UserTripSearchType.Price);
                    }
                    PriceVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _purchaseMonthVisibility = Visibility.Collapsed;

        public Visibility PurchaseMonthVisibility
        {
            get { return _purchaseMonthVisibility; }
            set { _purchaseMonthVisibility = value; OnPropertyChanged(); }
        }

        private bool _isPurchaseMonthChecked;

        public bool IsPurchaseMonthChecked
        {
            get { return _isPurchaseMonthChecked; }
            set 
            {
                _isPurchaseMonthChecked = value; 
                OnPropertyChanged(); 
                if (_isPurchaseMonthChecked)
                {
                    _searchTypes.Add(UserTripSearchType.PurchaseMonth);
                    PurchaseMonthVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(UserTripSearchType.PurchaseMonth))
                    {
                        _searchTypes.Remove(UserTripSearchType.PurchaseMonth);
                    }
                    PurchaseMonthVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _tripVisibility = Visibility.Collapsed;

        public Visibility TripVisibility
        {
            get { return _tripVisibility ; }
            set { _tripVisibility = value; OnPropertyChanged(); }
        }

        private bool _isTripChecked;

        public bool IsTripChecked
        {
            get { return _isTripChecked; }
            set 
            {
                _isTripChecked = value; 
                OnPropertyChanged(); 
                if (_isTripChecked)
                {
                    _searchTypes.Add(UserTripSearchType.Trip);
                    TripVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(UserTripSearchType.Trip))
                    {
                        _searchTypes.Remove(UserTripSearchType.Trip);
                    }
                    TripVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _criteriaForAgentVisibility;

        public Visibility CriteriaForAgentVisibility
        {
            get { return _criteriaForAgentVisibility; }
            set { _criteriaForAgentVisibility = value; OnPropertyChanged(); }
        }

        private readonly UserTripService _userTripService;

        public UserTripsViewModel UserTripViewModel { get; set; }

        public ICommand SearchCommand { get; }
        public ICommand CloseCommand { get; }

        public UserTripSearchViewModel(
            UserTripService userTripService)
        {
            _searchTypes = new HashSet<UserTripSearchType>();
            CriteriaForAgentVisibility = MainViewModel.SignedUser?.Type == UserType.Agent ? Visibility.Visible : Visibility.Collapsed;

            _userTripService = userTripService;

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
            IsPurchaseMonthChecked = false;
            IsTripChecked = false;
            _searchTypes.Clear();
            UserTripSearchModel = new Model.UserTripSearchModel();
        }

        private async void OnSearch(object o)
        {
            if (_searchTypes.Count > 0)
            {
                IEnumerable<Model.UserTripModel> userTrips = await _userTripService.Search(_searchTypes, UserTripSearchModel);
                UserTripViewModel.UserTrips.Clear();
                foreach (UserTripModel userTrip in userTrips)
                {
                    UserTripViewModel.UserTrips.Add(userTrip);
                }
            }
            else
            {
                if (MainViewModel.SignedUser?.Type == UserType.Traveler)
                {
                    await UserTripViewModel.LoadForUser();
                }
                else
                {
                    await UserTripViewModel.LoadAll();
                }

            }
            OnClose(this);
        }

        private bool CanSearch(object o)
        {
            bool canSearch = true;
            if (IsDepartureChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(UserTripSearchModel.DepartureSearchKeyword);
            }
            if (IsDestinationChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(UserTripSearchModel.DestinationSearchKeyword);
            }
            if (IsDepartureDateChecked)
            {
                canSearch = canSearch && (UserTripSearchModel.SelectedDepartureDate != null);
            }
            if (IsArrivalDateChecked)
            {
                canSearch = canSearch && (UserTripSearchModel.SelectedArrivalDate != null);
            }
            if (IsPurchaseMonthChecked)
            {
                canSearch = canSearch && (UserTripSearchModel.SelectedMonthIndex != null);
            }

            return canSearch;
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<UserTripSearchPopup>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();

            SetValuesToDefault();
        }

    }
}
