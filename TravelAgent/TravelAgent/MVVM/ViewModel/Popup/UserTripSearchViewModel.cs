using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<UserTripSearchType> AllSearchTypes { get; set; }
        public ObservableCollection<UserTripSearchType> SearchTypes { get; set; }

        private UserTripSearchType _selectedSearchType;

        public UserTripSearchType SelectedSearchType
        {
            get { return _selectedSearchType; }
            set 
            { 
                _selectedSearchType = value; 
                OnPropertyChanged(); 
            }
        }

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

        private Visibility _destinationVisibility = Visibility.Collapsed;

        public Visibility DestinationVisibility
        {
            get { return _destinationVisibility; }
            set { _destinationVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _departureDateVisibility = Visibility.Collapsed;

        public Visibility DepartureDateVisibility
        {
            get { return _departureDateVisibility; }
            set { _departureDateVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _arrivalDateVisibility = Visibility.Collapsed;

        public Visibility ArrivalDateVisibility
        {
            get { return _arrivalDateVisibility; }
            set { _arrivalDateVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _priceVisibility = Visibility.Collapsed;

        public Visibility PriceVisibility
        {
            get { return _priceVisibility; }
            set { _priceVisibility = value; OnPropertyChanged(); }
        }
        
        private Visibility _purchaseMonthVisibility = Visibility.Collapsed;

        public Visibility PurchaseMonthVisibility
        {
            get { return _purchaseMonthVisibility; }
            set { _purchaseMonthVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _tripVisibility = Visibility.Collapsed;

        public Visibility TripVisibility
        {
            get { return _tripVisibility ; }
            set { _tripVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _criteriaForAgentVisibility;

        public Visibility CriteriaForAgentVisibility
        {
            get { return _criteriaForAgentVisibility; }
            set { _criteriaForAgentVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _resetSearchVisibility = Visibility.Collapsed;

        public Visibility ResetSearchVisibility
        {
            get { return _resetSearchVisibility; }
            set { _resetSearchVisibility = value; OnPropertyChanged(); }
        }

        private readonly UserTripService _userTripService;

        public UserTripsViewModel UserTripViewModel { get; set; }

        private bool _searchCommandRunning = false;

        public ICommand SearchCommand { get; }
        public ICommand ResetSearchCommand { get; }
        public ICommand AddSearchTypeCommand { get; }
        public ICommand RemoveSearchTypeCommand { get; }
        public ICommand CloseCommand { get; }

        public UserTripSearchViewModel(
            UserTripService userTripService)
        {
            AllSearchTypes = new ObservableCollection<UserTripSearchType>()
            {
                UserTripSearchType.Departure,
                UserTripSearchType.Destination,
                UserTripSearchType.DepartureDateTime,
                UserTripSearchType.ArrivalDateTime,
                UserTripSearchType.Price,
                UserTripSearchType.PurchaseMonth,
                UserTripSearchType.Trip,
            };
            SelectedSearchType = AllSearchTypes.FirstOrDefault();
            SearchTypes = new ObservableCollection<UserTripSearchType>();
            CriteriaForAgentVisibility = MainViewModel.SignedUser?.Type == UserType.Agent ? Visibility.Visible : Visibility.Collapsed;

            _userTripService = userTripService;

            SearchCommand = new RelayCommand(OnSearch, CanSearch);
            ResetSearchCommand = new RelayCommand(OnResetSearch, o => true);
            AddSearchTypeCommand = new RelayCommand(OnAddSearchType, o => !SearchTypes.Contains(SelectedSearchType));
            RemoveSearchTypeCommand = new RelayCommand(OnRemoveSearchType, CanRemoveSearchType);
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private void OnAddSearchType(object o)
        {
            SearchTypes.Add(SelectedSearchType);
            switch (SelectedSearchType)
            {
                case UserTripSearchType.Departure:
                    DepartureVisibility = Visibility.Visible;
                    break;
                case UserTripSearchType.Destination:
                    DestinationVisibility = Visibility.Visible;
                    break;
                case UserTripSearchType.DepartureDateTime:
                    DepartureDateVisibility = Visibility.Visible;
                    break;
                case UserTripSearchType.ArrivalDateTime:
                    ArrivalDateVisibility = Visibility.Visible;
                    break;
                case UserTripSearchType.Price:
                    PriceVisibility = Visibility.Visible;
                    break;
                case UserTripSearchType.PurchaseMonth:
                    PurchaseMonthVisibility = Visibility.Visible;
                    break;
                case UserTripSearchType.Trip:
                    TripVisibility = Visibility.Visible;
                    break;
            }
        }

        private bool CanRemoveSearchType(object o)
        {
            if (o is UserTripSearchType searchType)
            {
                return SearchTypes.Contains(searchType);
            }

            return false;
        }

        private void OnRemoveSearchType(object o)
        {

            if (o is UserTripSearchType searchType)
            {
                SearchTypes.Remove(searchType);
                switch (searchType)
                {
                    case UserTripSearchType.Departure:
                        DepartureVisibility = Visibility.Collapsed;
                        break;
                    case UserTripSearchType.Destination:
                        DestinationVisibility = Visibility.Collapsed;
                        break;
                    case UserTripSearchType.DepartureDateTime:
                        DepartureDateVisibility = Visibility.Collapsed;
                        break;
                    case UserTripSearchType.ArrivalDateTime:
                        ArrivalDateVisibility = Visibility.Collapsed;
                        break;
                    case UserTripSearchType.Price:
                        PriceVisibility = Visibility.Collapsed;
                        break;
                    case UserTripSearchType.PurchaseMonth:
                        PurchaseMonthVisibility = Visibility.Collapsed;
                        break;
                    case UserTripSearchType.Trip:
                        TripVisibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        private void SetValuesToDefault()
        {
            SelectedSearchType = AllSearchTypes[0];
            DepartureVisibility = Visibility.Collapsed;
            DestinationVisibility = Visibility.Collapsed;
            DepartureDateVisibility = Visibility.Collapsed;
            ArrivalDateVisibility = Visibility.Collapsed;
            PriceVisibility = Visibility.Collapsed;
            PurchaseMonthVisibility = Visibility.Collapsed;
            TripVisibility = Visibility.Collapsed;
            SearchTypes.Clear();
            UserTripSearchModel = new Model.UserTripSearchModel();
        }

        private async void OnResetSearch(object o)
        {
            if (MainViewModel.SignedUser?.Type == UserType.Traveler)
            {
                await UserTripViewModel.LoadForUser();
            }
            else
            {
                await UserTripViewModel.LoadAll();
            }
            ResetSearchVisibility = Visibility.Collapsed;
            OnClose(this);
        }

        private async void OnSearch(object o)
        {
            _searchCommandRunning = true;

            if (SearchTypes.Count > 0)
            {
                IEnumerable<Model.UserTripModel> userTrips = await _userTripService.Search(SearchTypes, UserTripSearchModel);
                UserTripViewModel.UserTrips.Clear();
                foreach (UserTripModel userTrip in userTrips)
                {
                    UserTripViewModel.UserTrips.Add(userTrip);
                }
                ResetSearchVisibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("No criteria selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _searchCommandRunning = false;
                return;
            }

            _searchCommandRunning = false;

            OnClose(this);
        }

        private bool CanSearch(object o)
        {
            bool canSearch = true;
            if (SearchTypes.Contains(UserTripSearchType.Departure))
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(UserTripSearchModel.DepartureSearchKeyword);
            }
            if (SearchTypes.Contains(UserTripSearchType.Destination))
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(UserTripSearchModel.DestinationSearchKeyword);
            }
            if (SearchTypes.Contains(UserTripSearchType.DepartureDateTime))
            {
                canSearch = canSearch && (UserTripSearchModel.SelectedDepartureDate != null);
            }
            if (SearchTypes.Contains(UserTripSearchType.ArrivalDateTime))
            {
                canSearch = canSearch && (UserTripSearchModel.SelectedArrivalDate != null);
            }
            if (SearchTypes.Contains(UserTripSearchType.PurchaseMonth))
            {
                canSearch = canSearch && (UserTripSearchModel.SelectedMonthIndex != null);
            }

            return canSearch && !_searchCommandRunning;
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<UserTripSearchPopup>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();

            SetValuesToDefault();
        }

    }
}
