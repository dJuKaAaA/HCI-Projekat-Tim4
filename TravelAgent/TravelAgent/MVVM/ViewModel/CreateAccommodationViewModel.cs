using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class CreateAccommodationViewModel : Core.CreationViewModel
    {
        private AccommodationModel? _accommodationForModification;

        public AccommodationModel? AccommodationForModification
        {
            get { return _accommodationForModification; }
            set { _accommodationForModification = value; OnPropertyChanged(); }
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

        private string _rating = "0";

        public string Rating
        {
            get { return _rating; }
            set
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        _rating = "0";
                    }

                    double.Parse(value);

                    if (_rating == "0" && value.Length > 1)
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
                    _rating = value; 
                    OnPropertyChanged(); 
                }
                catch (FormatException) { }
            }
        }

        private PickLocationPopup? _pickLocationPopup;

        private readonly Consts _consts;
        private readonly NavigationService _navigationService;
        private readonly AccommodationService _accommodationService;
        private readonly LocationService _locationService;
        private readonly ImageService _imageService;

        public ICommand OpenLocationPickerCommand { get; }
        public ICommand CreateAccommodationCommand { get; }

        public CreateAccommodationViewModel(
            Consts consts,
            NavigationService navigationService,
            AccommodationService accommodationService,
            LocationService locationService,
            MapService mapService,
            ImageService imageService) : base(mapService)
        {
            _consts = consts;
            _navigationService = navigationService;
            _accommodationService = accommodationService;
            _locationService = locationService;
            _imageService = imageService;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenLocationPickerCommand = new RelayCommand(OnOpenLocationPicker, o => true);
            CreateAccommodationCommand = new RelayCommand(OnCreateAccommodation, CanCreateAccommodation);
            ClosePopupCommand = new RelayCommand(o => _pickLocationPopup?.Close(), o => _pickLocationPopup != null);


        }

        private async void OnCreateAccommodation(object o)
        {
            double rating = double.Parse(Rating);
            if (rating < 0 || rating > 5.0)
            {
                MessageBox.Show("Rating must be between 0 and 5!", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }

            if (!Modifying)
            {
                LocationModel location = await _locationService.Create(Location);

                AccommodationModel newAccommodation = new AccommodationModel()
                {
                    Name = Name,
                    Rating = rating,
                    Location = location,
                    Image = Image.UriSource.LocalPath
                };

                await _accommodationService.Create(newAccommodation);

                MessageBox.Show("Accommodation created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                SetDefaultValues();
            }
            else
            {
                LocationModel location = AccommodationForModification.Location;
                if (Location.Id != AccommodationForModification.Location.Id)
                {
                    location = await _locationService.Create(Location);
                }

                AccommodationModel modifiedAccommdation = new AccommodationModel()
                {
                    Name = Name,
                    Rating = rating,
                    Location = location,
                    Image = Image.UriSource.LocalPath
                };

                await _accommodationService.Modify(AccommodationForModification.Id, modifiedAccommdation);
                MessageBox.Show("Accommodation modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _navigationService.NavigateTo<AllAccommodationsViewModel>();
            }
        }

        private bool CanCreateAccommodation(object o)
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                Location != null;
        }

        private void SetDefaultValues()
        {
            Address = string.Empty;
            Location = null;
            Name = string.Empty;
            Rating = "0";
            Image = _imageService.GetFromLocalStorage($"{_consts.ProjectRootRelativePath}/Image/defaultimg.jpg");
        }

        private void SetValuesForModification()
        {
            Location = AccommodationForModification.Location;
            Name = AccommodationForModification.Name;
            Rating = AccommodationForModification.Rating.ToString();
            Image = _imageService.GetFromLocalStorage(AccommodationForModification.Image);
            Address = Location.Address;
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(CreateAccommodationViewModel))
            {
                _pickLocationPopup?.Close();
                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }

            if (e.Extra is AccommodationModel accommodation)
            {
                AccommodationForModification = accommodation;
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
