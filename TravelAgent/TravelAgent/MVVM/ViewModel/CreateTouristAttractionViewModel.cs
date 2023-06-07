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
    public class CreateTouristAttractionViewModel : Core.CreationViewModel
    {
        private TouristAttractionModel? _touristAttractionForModification;

        public TouristAttractionModel? TouristAttractionForModification
        {
            get { return _touristAttractionForModification; }
            set { _touristAttractionForModification = value; OnPropertyChanged(); }
        }

        private bool _modifying;

        public bool Modifying
        {
            get { return _modifying; }
            set { _modifying = value; OnPropertyChanged(); }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
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
        private readonly TouristAttractionService _touristAttractionService;
        private readonly LocationService _locationService;
        private readonly ImageService _imageService;

        private bool _createTouristAttractionCommandRunning = false;

        public ICommand OpenLocationPickerCommand { get; }
        public ICommand CreateTouristAttractionCommand { get; }

        public CreateTouristAttractionViewModel(
            Consts consts,
            NavigationService navigationService,
            TouristAttractionService touristAttractionService,
            LocationService locationService,
            MapService mapService,
            ImageService imageService) : base(mapService)
        {
            _consts = consts;
            _navigationService = navigationService;
            _touristAttractionService = touristAttractionService;
            _locationService = locationService;
            _imageService = imageService;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenLocationPickerCommand = new RelayCommand(OnOpenLocationPicker, o => true);
            CreateTouristAttractionCommand = new RelayCommand(OnCreateTouristAttraction, CanCreateTouristAttraction);
            ClosePopupCommand = new RelayCommand(o => _pickLocationPopup?.Close(), o => _pickLocationPopup != null);

        }

        private async void OnCreateTouristAttraction(object o)
        {
            _createTouristAttractionCommandRunning = true;

            if (!Modifying)
            {
                LocationModel location = await _locationService.Create(Location);

                TouristAttractionModel newTouristAttraction = new TouristAttractionModel()
                {
                    Name = Name,
                    Location = location,
                    Image = Image.UriSource.LocalPath
                };

                await _touristAttractionService.Create(newTouristAttraction);

                MessageBox.Show("Tourist attraction created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                int locationForDeletionId = 0;
                LocationModel location = TouristAttractionForModification.Location;
                if (Location.Id != TouristAttractionForModification.Location.Id)
                {
                    locationForDeletionId = TouristAttractionForModification.Location.Id;
                    location = await _locationService.Create(Location);
                }

                TouristAttractionModel modifiedTouristAttraction = new TouristAttractionModel()
                {
                    Name = Name,
                    Location = location,
                    Image = Image.UriSource.LocalPath
                };

                await _touristAttractionService.Modify(TouristAttractionForModification.Id, modifiedTouristAttraction);
                if (locationForDeletionId != 0)
                {
                    await _locationService.Delete(locationForDeletionId);
                }

                MessageBox.Show("Tourist attraction modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _createTouristAttractionCommandRunning = false;
            _navigationService.NavigateTo<AllTouristAttractionsViewModel>();
        }

        private bool CanCreateTouristAttraction(object o)
        {
            bool canCreateTouristAttraction = !string.IsNullOrWhiteSpace(Name) &&
                Location != null &&
                !_createTouristAttractionCommandRunning;
            if (canCreateTouristAttraction)
            {
                FinalizationWarningVisibility = Visibility.Collapsed;
            }
            else
            {
                FinalizationWarningVisibility = Visibility.Visible;
            }
            return canCreateTouristAttraction;
        }

        private void SetDefaultValues()
        {
            Address = string.Empty;
            Location = null;
            Name = string.Empty;
            Image = _imageService.GetFromLocalStorage($"{_consts.ProjectRootRelativePath}/Image/defaultimg.jpg");
        }

        private void SetValuesForModification()
        {
            Location = TouristAttractionForModification.Location;
            Name = TouristAttractionForModification.Name;
            Image = _imageService.GetFromLocalStorage(TouristAttractionForModification.Image);
            Address = Location.Address;
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(CreateAccommodationViewModel))
            {
                _pickLocationPopup?.Close();
                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }

            if (e.Extra is TouristAttractionModel touristAttraction)
            {
                TouristAttractionForModification = touristAttraction;
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
