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

namespace TravelAgent.MVVM.ViewModel
{
    public class CreateRestaurantViewModel : Core.CreationViewModel
    {
        private RestaurantModel? _restaurantForModification;

        public RestaurantModel? RestaurantForModification
        {
            get { return _restaurantForModification; }
            set { _restaurantForModification = value; OnPropertyChanged(); }
        }

        private bool _modifying;

        public bool Modifying
        {
            get { return _modifying; }
            set { _modifying = value; OnPropertyChanged(); }
        }

        private bool _isLocationChanged;

        public bool IsLocationChanged
        {
            get { return _isLocationChanged; }
            set { _isLocationChanged = value; OnPropertyChanged(); }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private int _stars = 1;

        public int Stars
        {
            get { return _stars; }
            set { _stars =  value; OnPropertyChanged(); }
        }

        private Visibility _finalizationWarningVisibility = Visibility.Visible;

        public Visibility FinalizationWarningVisibility
        {
            get { return _finalizationWarningVisibility; }
            set { _finalizationWarningVisibility = value; OnPropertyChanged(); }
        }

        private PickLocationPopup? _pickLocationPopup;

        private readonly Consts _consts;
        private readonly NavigationService _navigationService;
        private readonly RestaurantService _restaurantService;
        private readonly LocationService _locationService;
        private readonly ImageService _imageService;

        private bool _createRestaurantCommandRunning = false;

        public ICommand OpenLocationPickerCommand { get; }
        public ICommand CreateRestaurantCommand { get; }
        public ICommand SelectStarsCommand { get; }

        public CreateRestaurantViewModel(
            Consts consts,
            NavigationService navigationService,
            RestaurantService restaurantService,
            LocationService locationService,
            MapService mapService,
            ImageService imageService) : base(mapService)
        {
            _consts = consts;
            _navigationService = navigationService;
            _restaurantService = restaurantService;
            _locationService = locationService;
            _imageService = imageService;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenLocationPickerCommand = new RelayCommand(OnOpenLocationPicker, o => true);
            CreateRestaurantCommand = new RelayCommand(OnCreateRestaurant, CanCreateRestaurant);
            SelectStarsCommand = new RelayCommand(OnSelectStars, o => true);
            ClosePopupCommand = new RelayCommand(o => _pickLocationPopup?.Close(), o => _pickLocationPopup != null);

        }

        private void OnSelectStars(object o)
        {
            int star;
            if (int.TryParse(o.ToString(), out star))
            {
                Stars = star;
            }
        }

        private async void OnCreateRestaurant(object o)
        {
            _createRestaurantCommandRunning = true;

            if (!Modifying)
            {
                LocationModel location = await _locationService.Create(Location);

                RestaurantModel newRestaurant = new RestaurantModel()
                {
                    Name = Name,
                    Stars = Stars,
                    Location = location,
                    Image = Image.UriSource.LocalPath
                };

                await _restaurantService.Create(newRestaurant);

                MessageBox.Show("Restaurant created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                int locationForDeletionId = 0;
                LocationModel location = RestaurantForModification.Location;
                if (Location.Id != RestaurantForModification.Location.Id)
                {
                    locationForDeletionId = RestaurantForModification.Location.Id;
                    location = await _locationService.Create(Location);
                }

                RestaurantModel modifiedRestaurant = new RestaurantModel()
                {
                    Name = Name,
                    Stars = Stars,
                    Location = location,
                    Image = Image.UriSource.LocalPath
                };

                await _restaurantService.Modify(RestaurantForModification.Id, modifiedRestaurant);
                if (locationForDeletionId != 0)
                {
                    await _locationService.Delete(locationForDeletionId);
                }

                MessageBox.Show("Restaurant modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _createRestaurantCommandRunning = false;
            _navigationService.NavigateTo<AllRestaurantsViewModel>();
        }

        private bool CanCreateRestaurant(object o)
        {
            bool canCreateRestaurant = !string.IsNullOrWhiteSpace(Name) &&
                Location != null &&
                !_createRestaurantCommandRunning;
            if (canCreateRestaurant)
            {
                FinalizationWarningVisibility = Visibility.Collapsed;
            }
            else
            {
                FinalizationWarningVisibility = Visibility.Visible;
            }
            return canCreateRestaurant;
        }

        private void SetDefaultValues()
        {
            Address = string.Empty;
            Location = null;
            Name = string.Empty;
            Stars = 1;
            Image = _imageService.GetFromLocalStorage($"{_consts.ProjectRootRelativePath}/Image/defaultimg.jpg");
        }

        private void SetValuesForModification()
        {
            Location = RestaurantForModification.Location;
            Name = RestaurantForModification.Name;
            Stars = RestaurantForModification.Stars;
            Image = _imageService.GetFromLocalStorage(RestaurantForModification.Image);
            Address = Location.Address;
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(CreateAccommodationViewModel))
            {
                _pickLocationPopup?.Close();
                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }

            if (e.Extra is RestaurantModel restaurant)
            {
                RestaurantForModification = restaurant;
                Modifying = true;
                SetValuesForModification();
            }
            else
            {
                SetDefaultValues();
            }
        }

        private void OnOpenLocationPicker(object o)
        {
            _pickLocationPopup?.Close();
            _pickLocationPopup = new PickLocationPopup()
            {
                DataContext = this
            };
            if (Location != null)
            {
                _pickLocationPopup.PickedLocationPushpin = MapService.CreatePushpin( 
                    Location.Latitude, 
                    Location.Longitude, 
                    Location.Address, 
                    "PickedLocation");
            }
            _pickLocationPopup.Show();
        }

    }
}
