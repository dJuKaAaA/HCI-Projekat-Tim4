using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.MVVM.View.Popup;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel.Popup
{
    public class RestaurantSearchViewModel : Core.ViewModel
    {
        private HashSet<RestaurantSearchType> _searchTypes;

        private Model.RestaurantSearchModel _restaurantSearchModel = new Model.RestaurantSearchModel();

        public Model.RestaurantSearchModel RestaurantSearchModel
        {
            get { return _restaurantSearchModel; }
            set { _restaurantSearchModel = value; OnPropertyChanged(); }
        }

        private Visibility _nameVisibility = Visibility.Collapsed;

        public Visibility NameVisibility
        {
            get { return _nameVisibility; }
            set { _nameVisibility = value; OnPropertyChanged(); }
        }

        private bool _isNameChecked;

        public bool IsNameChecked
        {
            get { return _isNameChecked; }
            set 
            { 
                _isNameChecked = value; 
                OnPropertyChanged(); 
                if (_isNameChecked)
                {
                    _searchTypes.Add(RestaurantSearchType.Name);
                    NameVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(RestaurantSearchType.Name))
                    {
                        _searchTypes.Remove(RestaurantSearchType.Name);
                    }
                    NameVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _addressVisibility = Visibility.Collapsed;

        public Visibility AddressVisibility
        {
            get { return _addressVisibility; }
            set { _addressVisibility = value; OnPropertyChanged(); }
        }

        private bool _isAddressChecked;

        public bool IsAddressChecked
        {
            get { return _isAddressChecked; }
            set 
            {
                _isAddressChecked = value; 
                OnPropertyChanged(); 
                if (_isAddressChecked)
                {
                    _searchTypes.Add(RestaurantSearchType.Address);
                    AddressVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(RestaurantSearchType.Address))
                    {
                        _searchTypes.Remove(RestaurantSearchType.Address);
                    }
                    AddressVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _starsVisibility = Visibility.Collapsed;

        public Visibility StarsVisibility
        {
            get { return _starsVisibility; }
            set { _starsVisibility = value; OnPropertyChanged(); }
        }

        private bool _isStarsChecked;

        public bool IsStarsChecked
        {
            get { return _isStarsChecked; }
            set 
            { 
                _isStarsChecked = value; 
                OnPropertyChanged(); 
                if (_isStarsChecked)
                {
                    _searchTypes.Add(RestaurantSearchType.Stars);
                    StarsVisibility = Visibility.Visible;
                }
                else
                {
                    if (_searchTypes.Contains(RestaurantSearchType.Stars))
                    {
                        _searchTypes.Remove(RestaurantSearchType.Stars);
                    }
                    StarsVisibility = Visibility.Collapsed;
                }
            }
        }

        private Visibility _resetSearchVisibility = Visibility.Collapsed;

        public Visibility ResetSearchVisibility
        {
            get { return _resetSearchVisibility; }
            set { _resetSearchVisibility = value; OnPropertyChanged(); }
        }

        private readonly RestaurantService _restaurantService;

        public AllRestaurantsViewModel AllRestaurantsViewModel { get; set; }

        private bool _searchCommandRunning = false;

        public ICommand SearchCommand { get; }
        public ICommand ResetSearchCommand { get; }
        public ICommand SelectStarsCommand { get; }
        public ICommand CloseCommand { get; }

        public RestaurantSearchViewModel(
            RestaurantService restaurantService)
        {
            _searchTypes = new HashSet<RestaurantSearchType>();

            _restaurantService = restaurantService;

            SearchCommand = new RelayCommand(OnSearch, CanSearch);
            ResetSearchCommand = new RelayCommand(OnResetSearch, o => true);
            SelectStarsCommand = new RelayCommand(OnSelectStars, o => true);
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private void SetValuesToDefault()
        {
            IsNameChecked = false;
            IsAddressChecked = false;
            IsStarsChecked = false;
            _searchTypes.Clear();
            RestaurantSearchModel = new Model.RestaurantSearchModel();
        }

        private void OnSelectStars(object o)
        {
            int star;
            if (int.TryParse(o.ToString(), out star))
            {
                RestaurantSearchModel.Stars = star;
            }
        }

        private async void OnResetSearch(object o)
        {
            await AllRestaurantsViewModel.LoadAll();
            ResetSearchVisibility = Visibility.Collapsed;
            OnClose(this);
        }

        private async void OnSearch(object o)
        {
            _searchCommandRunning = true;
            
            if (_searchTypes.Count > 0)
            {
                IEnumerable<Model.RestaurantModel> restaurants = await _restaurantService.Search(_searchTypes, RestaurantSearchModel);
                AllRestaurantsViewModel.Restaurants.Clear();
                foreach (Model.RestaurantModel restaurant in restaurants)
                {
                    AllRestaurantsViewModel.Restaurants.Add(restaurant);
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
            if (IsNameChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(RestaurantSearchModel.NameKeyword);
            }
            if (IsAddressChecked)
            {
                canSearch = canSearch && !string.IsNullOrWhiteSpace(RestaurantSearchModel.AddressKeyword);
            }

            return canSearch && !_searchCommandRunning;
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<RestaurantSearchPopup>().SingleOrDefault(w => w.IsActive);
            currentWindow?.Close();

            SetValuesToDefault();
        }
    }
}
