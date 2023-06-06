using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<TripSearchType> AllSearchTypes { get; set; }
        public ObservableCollection<TripSearchType> SearchTypes { get; set; }

        private TripSearchType _selectedSearchType;

        public TripSearchType SelectedSearchType
        {
            get { return _selectedSearchType; }
            set 
            { 
                _selectedSearchType = value; 
                OnPropertyChanged(); 
            }
        }

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

        private Visibility _resetSearchVisibility = Visibility.Collapsed;

        public Visibility ResetSearchVisibility
        {
            get { return _resetSearchVisibility; }
            set { _resetSearchVisibility = value; OnPropertyChanged(); }
        }

        private readonly TripService _tripService;

        public AllTripsViewModel AllTripsViewModel { get; set; }

        private bool _searchCommandRunning = false;

        public ICommand SearchCommand { get; }
        public ICommand ResetSearchCommand { get; }
        public ICommand AddSearchTypeCommand { get; }
        public ICommand RemoveSearchTypeCommand { get; }
        public ICommand CloseCommand { get; }

        public TripSearchViewModel(
            TripService tripService)
        {
            AllSearchTypes = new ObservableCollection<TripSearchType>()
            {
                TripSearchType.Departure,
                TripSearchType.Destination,
                TripSearchType.DepartureDateTime,
                TripSearchType.ArrivalDateTime,
                TripSearchType.Price,
            };
            SelectedSearchType = AllSearchTypes.FirstOrDefault();
            SearchTypes = new ObservableCollection<TripSearchType>();

            _tripService = tripService;

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
                case TripSearchType.Departure:
                    DepartureVisibility = Visibility.Visible;
                    break;
                case TripSearchType.Destination:
                    DestinationVisibility = Visibility.Visible;
                    break;
                case TripSearchType.DepartureDateTime:
                    DepartureDateVisibility = Visibility.Visible;
                    break;
                case TripSearchType.ArrivalDateTime:
                    ArrivalDateVisibility = Visibility.Visible;
                    break;
                case TripSearchType.Price:
                    PriceVisibility = Visibility.Visible;
                    break;
            }
        }

        private bool CanRemoveSearchType(object o)
        {
            if (o is TripSearchType searchType)
            {
                return SearchTypes.Contains(searchType);
            }

            return false;
        }

        private void OnRemoveSearchType(object o)
        {

            if (o is TripSearchType searchType)
            {
                SearchTypes.Remove(searchType);
                switch (searchType)
                {
                    case TripSearchType.Departure:
                        DepartureVisibility = Visibility.Collapsed;
                        break;
                    case TripSearchType.Destination:
                        DestinationVisibility = Visibility.Collapsed;
                        break;
                    case TripSearchType.DepartureDateTime:
                        DepartureDateVisibility = Visibility.Collapsed;
                        break;
                    case TripSearchType.ArrivalDateTime:
                        ArrivalDateVisibility = Visibility.Collapsed;
                        break;
                    case TripSearchType.Price:
                        PriceVisibility = Visibility.Collapsed;
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
            SearchTypes.Clear();
            TripSearchModel = new Model.TripSearchModel();
        }

        private async void OnResetSearch(object o)
        {
            await AllTripsViewModel.LoadAll();
            ResetSearchVisibility = Visibility.Collapsed;
            OnClose(this);
        }

        private async void OnSearch(object o)
        {
            _searchCommandRunning = true;

            if (SearchTypes.Count > 0)
            {
                IEnumerable<Model.TripModel> trips = await _tripService.Search(SearchTypes, TripSearchModel);
                AllTripsViewModel.Trips.Clear();
                foreach (TripModel trip in trips)
                {
                    AllTripsViewModel.Trips.Add(trip);
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
            if (SearchTypes.Contains(TripSearchType.Departure))
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(TripSearchModel.DepartureSearchKeyword);
            }
            if (SearchTypes.Contains(TripSearchType.Destination))
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(TripSearchModel.DestinationSearchKeyword);
            }
            if (SearchTypes.Contains(TripSearchType.DepartureDateTime))
            {
                canSearch = canSearch && (TripSearchModel.SelectedDepartureDate != null);
            }
            if (SearchTypes.Contains(TripSearchType.ArrivalDateTime))
            {
                canSearch = canSearch && (TripSearchModel.SelectedArrivalDate != null);
            }

            return canSearch && !_searchCommandRunning;
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<TripSearchPopup>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();

            SetValuesToDefault();
        }
    }
}
