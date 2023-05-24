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
using System.Windows.Shapes;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.ViewModel.Popup;
using TravelAgent.Service;

namespace TravelAgent.MVVM.View.Popup
{
    /// <summary>
    /// Interaction logic for SeeDealPopup.xaml
    /// </summary>
    public partial class SeeDealPopup : Window
    {

        private SeeDealViewModel _viewModel;

        public SeeDealPopup(
            TripModel trip,
            UserTripService userTripService,
            MapService mapService,
            TouristAttractionService touristAttractionService,
            RestorauntService restorauntService,
            AccommodationService accommodationService,
            Consts consts)
        {
            InitializeComponent();
            _viewModel = ((SeeDealViewModel)DataContext);

            _viewModel.Trip = trip;
            _viewModel.UserTripService = userTripService;
            _viewModel.MapService = mapService;
            _viewModel.TouristAttractionService = touristAttractionService;
            _viewModel.RestorauntService = restorauntService;
            _viewModel.AccommodationsService = accommodationService;
            _viewModel.Consts = consts;
            DateTime takeoff = _viewModel.Trip.DepartureDateTime;
            DateTime landing = _viewModel.Trip.ArrivalDateTime;
            TimeSpan timeDiff = landing - takeoff;
            _viewModel.TripDuration = timeDiff.Hours;
        }

        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            DrawPointsOfInterestPushpins();

            MapService mapService = _viewModel.MapService;

            TripModel trip = _viewModel.Trip;

            Pushpin departurePushpin = mapService.CreatePushpin(
                trip.Departure.Latitude,
                trip.Departure.Longitude,
                trip.Departure.Address,
                $"Departure_{trip.Departure.Id}");
            Pushpin destinationPushpin = mapService.CreatePushpin(
                trip.Destination.Latitude,
                trip.Destination.Longitude,
                trip.Destination.Address,
                $"Destination_{trip.Destination.Id}");

            MapPolyline line = mapService.CreatePushpinLine(departurePushpin.Location, destinationPushpin.Location);

            mapControl.Children.Add(departurePushpin);
            mapControl.Children.Add(destinationPushpin);
            mapControl.Children.Add(line);

            mapControl.Center = departurePushpin.Location;
        }

        private async void DrawPointsOfInterestPushpins()
        {
            MapService mapService = _viewModel.MapService;
            Consts consts = _viewModel.Consts;

            // Draw tourist attractions
            await _viewModel.LoadTouristAttractionsForTrip();
            foreach (TouristAttractionModel touristAttraction in _viewModel.TouristAttractionsForTrip)
            {
                Pushpin touristAttractionPushpin = mapService.CreatePushpin(
                    touristAttraction.Location.Latitude,
                    touristAttraction.Location.Longitude,
                    touristAttraction.Name,
                    $"TouristAttraction_{touristAttraction.Id}",
                    $"{consts.PathToIcons}/{consts.TouristAttractionPushpinIcon}");
                mapControl.Children.Add(touristAttractionPushpin);
            }

            // Draw restoraunt
            await _viewModel.LoadRestorauntsForTrip();
            foreach (RestorauntModel restoraunt in _viewModel.RestorauntsForTrip)
            {
                Pushpin restorauntPushpin = mapService.CreatePushpin(
                    restoraunt.Location.Latitude,
                    restoraunt.Location.Longitude,
                    restoraunt.Name,
                    $"Restoraunt_{restoraunt.Id}",
                    $"{consts.PathToIcons}/{consts.RestorauntPushpinIcon}");
                mapControl.Children.Add(restorauntPushpin);
            }

            // Draw accommodations
            await _viewModel.LoadAccommodationsForTrip();
            foreach (AccommodationModel accommodation in _viewModel.AccommodationsForTrip)
            {
                Pushpin accommodationPushpin = mapService.CreatePushpin(
                    accommodation.Location.Latitude,
                    accommodation.Location.Longitude,
                    accommodation.Name,
                    $"Restoraunt_{accommodation.Id}",
                    $"{consts.PathToIcons}/{consts.AccommodationPushpinIcon}");
                mapControl.Children.Add(accommodationPushpin);
            }
        }
    }
}
