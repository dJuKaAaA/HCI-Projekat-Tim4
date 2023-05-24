using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.ViewModel;
using TravelAgent.Service;

namespace TravelAgent.MVVM.View
{
    /// <summary>
    /// Interaction logic for CreateTripView.xaml
    /// </summary>
    public partial class CreateTripView : UserControl
    {
        public CreateTripView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((CreateTripViewModel)DataContext).DepartureAddressSearched += OnDepartureAddressSearched;
            ((CreateTripViewModel)DataContext).DestinationAddressSearched += OnDestinationAddressSearched;
            DrawAccomodationsPushpins();
            DrawRestorauntsPushpins();
            DrawTouristAttractionPushpins();
        }

        private void OnDestinationAddressSearched(object? sender, LocationModel location)
        {
            Service.MapService mapService = ((CreateTripViewModel)DataContext).MapService;

            if (_departurePushpin == null)
            {
                _destinationPushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Destination");
                mapControl.Children.Add(_destinationPushpin);
                ((CreateTripViewModel)DataContext).SelectedDestinationLocation = location;
                ((CreateTripViewModel)DataContext).DestinationAddress = location.Address;
            }
            else
            {
                if (_destinationPushpin != null)
                {
                    mapControl.Children.Remove(_destinationPushpin);
                    mapControl.Children.Remove(_tripLine);
                    _tripLine = null;
                }

                _destinationPushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Destination");
                mapControl.Children.Add(_destinationPushpin);
                ((CreateTripViewModel)DataContext).SelectedDestinationLocation = location;
                ((CreateTripViewModel)DataContext).DestinationAddress = location.Address;

                _tripLine = mapService.CreatePushpinLine(_departurePushpin.Location, _destinationPushpin.Location);
                mapControl.Children.Add(_tripLine);
            }

            mapControl.Center = new Location(location.Latitude, location.Longitude);
            mapControl.ZoomLevel = 12;
        }

        private async void DrawRestorauntsPushpins()
        {
            Service.MapService mapService = ((CreateTripViewModel)DataContext).MapService;
            Consts consts = ((CreateTripViewModel)DataContext).Consts;

            await ((CreateTripViewModel)DataContext).LoadAllRestoraunts();

            foreach (RestorauntModel restoraunt in ((CreateTripViewModel)DataContext).AllRestoraunts)
            {
                Pushpin pushpin = mapService.CreatePushpin(
                    restoraunt.Location.Latitude,
                    restoraunt.Location.Longitude,
                    restoraunt.Name,
                    $"Restoraunt_{restoraunt.Id}",
                    $"{consts.PathToIcons}/{consts.RestorauntPushpin}");
                mapControl.Children.Add(pushpin);
            }

        }

        private async void DrawAccomodationsPushpins()
        {
            Service.MapService mapService = ((CreateTripViewModel)DataContext).MapService;
            Consts consts = ((CreateTripViewModel)DataContext).Consts;

            await ((CreateTripViewModel)DataContext).LoadAllAccommodations();

            foreach (AccommodationModel accommodation in ((CreateTripViewModel)DataContext).AllAccommodations)
            {
                Pushpin pushpin = mapService.CreatePushpin(
                    accommodation.Location.Latitude,
                    accommodation.Location.Longitude,
                    accommodation.Name,
                    $"Accommodation_{accommodation.Id}",
                    $"{consts.PathToIcons}/{consts.AccommodationPushpin}");
                mapControl.Children.Add(pushpin);
            }

        }

        private async void DrawTouristAttractionPushpins()
        {
            Service.MapService mapService = ((CreateTripViewModel)DataContext).MapService;
            Consts consts = ((CreateTripViewModel)DataContext).Consts;

            await ((CreateTripViewModel)DataContext).LoadAllTouristAttractions();

            foreach (TouristAttractionModel touristAttraction in ((CreateTripViewModel)DataContext).AllTouristAttractions)
            {
                Pushpin pushpin = mapService.CreatePushpin(
                    touristAttraction.Location.Latitude,
                    touristAttraction.Location.Longitude,
                    touristAttraction.Name,
                    $"TouristAttraction_{touristAttraction.Id}",
                    $"{consts.PathToIcons}/{consts.TouristAttractionPushpin}");
                mapControl.Children.Add(pushpin);
            }

        }

        private void OnDepartureAddressSearched(object? sender, LocationModel location)
        {
            Service.MapService mapService = ((CreateTripViewModel)DataContext).MapService;

            if (_destinationPushpin == null)
            {
                _departurePushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Departure");
                mapControl.Children.Add(_departurePushpin);
                ((CreateTripViewModel)DataContext).SelectedDepartureLocation = location;
                ((CreateTripViewModel)DataContext).DepartureAddress = location.Address;
            }
            else
            {
                if (_departurePushpin != null)
                {
                    mapControl.Children.Remove(_departurePushpin);
                    mapControl.Children.Remove(_tripLine);
                    _tripLine = null;
                }

                _departurePushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Departure");
                mapControl.Children.Add(_departurePushpin);
                ((CreateTripViewModel)DataContext).SelectedDepartureLocation = location;
                ((CreateTripViewModel)DataContext).DepartureAddress = location.Address;

                _tripLine = mapService.CreatePushpinLine(_departurePushpin.Location, _destinationPushpin.Location);
                mapControl.Children.Add(_tripLine);
            }
            
            mapControl.Center = new Location(location.Latitude, location.Longitude);
            mapControl.ZoomLevel = 12;
        }

        private Pushpin? _departurePushpin;
        private Pushpin? _destinationPushpin;
        private MapPolyline? _tripLine;

        private async void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Service.MapService mapService = ((CreateTripViewModel)DataContext).MapService;

            // Get the clicked location from the event arguments
            Point mousePosition = e.GetPosition((UIElement)sender);
            Location clickedLocation = mapControl.ViewportPointToLocation(mousePosition);

            LocationModel location = await mapService.ReverseGeocode(clickedLocation.Latitude, clickedLocation.Longitude);

            if (_departurePushpin == null)
            {
                _departurePushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Departure");
                mapControl.Children.Add(_departurePushpin);
                ((CreateTripViewModel)DataContext).SelectedDepartureLocation = location;
                ((CreateTripViewModel)DataContext).DepartureAddress = location.Address;
            }
            else if (_destinationPushpin == null)
            {
                _destinationPushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Destination");
                mapControl.Children.Add(_destinationPushpin);
                ((CreateTripViewModel)DataContext).SelectedDestinationLocation = location;
                ((CreateTripViewModel)DataContext).DestinationAddress = location.Address;

                _tripLine = mapService.CreatePushpinLine(_departurePushpin.Location, _destinationPushpin.Location);
                mapControl.Children.Add(_tripLine);
            }
            else
            {
                mapControl.Children.Remove(_departurePushpin);
                mapControl.Children.Remove(_destinationPushpin);
                mapControl.Children.Remove(_tripLine);

                _departurePushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Departure");
                _destinationPushpin = null;
                _tripLine = null;
                ((CreateTripViewModel)DataContext).SelectedDepartureLocation = location;
                ((CreateTripViewModel)DataContext).SelectedDestinationLocation = null;

                ((CreateTripViewModel)DataContext).DepartureAddress = location.Address;
                ((CreateTripViewModel)DataContext).DestinationAddress = string.Empty;

                mapControl.Children.Add(_departurePushpin);

            }
        }

    }
}
