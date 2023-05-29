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
    public class CreateRestorauntViewModel : Core.CreationViewModel
    {
        private RestorauntModel? _restorauntForModification;

        public RestorauntModel? RestorauntForModification
        {
            get { return _restorauntForModification; }
            set { _restorauntForModification = value; OnPropertyChanged(); }
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

        private PickLocationPopup? _pickLocationPopup;

        private readonly Consts _consts;
        private readonly NavigationService _navigationService;
        private readonly RestorauntService _restorauntService;
        private readonly LocationService _locationService;
        private readonly ImageService _imageService;

        public ICommand OpenLocationPickerCommand { get; }
        public ICommand CreateRestorauntCommand { get; }
        public ICommand SelectStarsCommand { get; }

        public CreateRestorauntViewModel(
            Consts consts,
            NavigationService navigationService,
            RestorauntService restorauntService,
            LocationService locationService,
            MapService mapService,
            ImageService imageService) : base(mapService)
        {
            _consts = consts;
            _navigationService = navigationService;
            _restorauntService = restorauntService;
            _locationService = locationService;
            _imageService = imageService;

            _navigationService.NavigationCompleted += OnNavigationCompleted;

            OpenLocationPickerCommand = new RelayCommand(OnOpenLocationPicker, o => true);
            CreateRestorauntCommand = new RelayCommand(OnCreateRestoraunt, CanCreateRestoraunt);
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

        private async void OnCreateRestoraunt(object o)
        {
            if (!Modifying)
            {
                LocationModel location = await _locationService.Create(Location);

                RestorauntModel newRestoraunt = new RestorauntModel()
                {
                    Name = Name,
                    Stars = Stars,
                    Location = location,
                    Image = Image.UriSource.LocalPath
                };

                await _restorauntService.Create(newRestoraunt);

                MessageBox.Show("Restoraunt created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                SetDefaultValues();
            }
            else
            {
                int locationForDeletionId = 0;
                LocationModel location = RestorauntForModification.Location;
                if (Location.Id != RestorauntForModification.Location.Id)
                {
                    locationForDeletionId = RestorauntForModification.Location.Id;
                    location = await _locationService.Create(Location);
                }

                RestorauntModel modifiedRestoraunt = new RestorauntModel()
                {
                    Name = Name,
                    Stars = Stars,
                    Location = location,
                    Image = Image.UriSource.LocalPath
                };

                await _restorauntService.Modify(RestorauntForModification.Id, modifiedRestoraunt);
                if (locationForDeletionId != 0)
                {
                    await _locationService.Delete(locationForDeletionId);
                }

                MessageBox.Show("Restoraunt modified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _navigationService.NavigateTo<AllRestorauntsViewModel>();
            }
        }

        private bool CanCreateRestoraunt(object o)
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                Location != null;
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
            Location = RestorauntForModification.Location;
            Name = RestorauntForModification.Name;
            Stars = RestorauntForModification.Stars;
            Image = _imageService.GetFromLocalStorage(RestorauntForModification.Image);
            Address = Location.Address;
        }

        private void OnNavigationCompleted(object? sender, NavigationEventArgs e)
        {
            if (e.ViewModelType != typeof(CreateAccommodationViewModel))
            {
                _pickLocationPopup?.Close();
                _navigationService.NavigationCompleted -= OnNavigationCompleted;
            }

            if (e.Extra is RestorauntModel restoraunt)
            {
                RestorauntForModification = restoraunt;
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
