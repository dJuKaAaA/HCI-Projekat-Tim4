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

            ((SeeDealViewModel)DataContext).Trip = trip;
            ((SeeDealViewModel)DataContext).UserTripService = userTripService;
            ((SeeDealViewModel)DataContext).MapService = mapService;
            ((SeeDealViewModel)DataContext).TouristAttractionService = touristAttractionService;
            ((SeeDealViewModel)DataContext).RestorauntService = restorauntService;
            ((SeeDealViewModel)DataContext).AccommodationsService = accommodationService;
            ((SeeDealViewModel)DataContext).Consts = consts;
            DateTime takeoff = ((SeeDealViewModel)DataContext).Trip.DepartureDateTime;
            DateTime landing = ((SeeDealViewModel)DataContext).Trip.ArrivalDateTime;
            TimeSpan timeDiff = landing - takeoff;
            ((SeeDealViewModel)DataContext).TripDuration = timeDiff.Hours;
        }

        private async void DrawPointsOfInterestPushpins()
        {
            MapService mapService = ((SeeDealViewModel)DataContext).MapService;
            Consts consts = ((SeeDealViewModel)DataContext).Consts;
            // TODO: Add special icons for each type of point of interest

            // drawing tourist attractions
            await ((SeeDealViewModel)DataContext).LoadTouristAttractionsForTrip();
            foreach (TouristAttractionModel touristAttraction in ((SeeDealViewModel)DataContext).TouristAttractionsForTrip)
            {
                Pushpin touristAttractionPushpin = mapService.CreatePushpin(
                    touristAttraction.Location.Latitude,
                    touristAttraction.Location.Longitude,
                    touristAttraction.Name,
                    $"TouristAttraction_{touristAttraction.Id}",
                    $"{consts.PathToIcons}/{consts.TouristAttractionPushpin}");
                mapControl.Children.Add(touristAttractionPushpin);
            }

            // TODO: Draw restoraunts
            await ((SeeDealViewModel)DataContext).LoadRestorauntsForTrip();
            foreach (RestorauntModel restoraunt in ((SeeDealViewModel)DataContext).RestorauntsForTrip)
            {
                Pushpin restorauntPushpin = mapService.CreatePushpin(
                    restoraunt.Location.Latitude,
                    restoraunt.Location.Longitude,
                    restoraunt.Name,
                    $"Restoraunt_{restoraunt.Id}",
                    $"{consts.PathToIcons}/{consts.RestorauntPushpin}");
                mapControl.Children.Add(restorauntPushpin);
            }

            // TODO: Draw accommodations
            await ((SeeDealViewModel)DataContext).LoadAccommodationsForTrip();
            foreach (AccommodationModel accommodation in ((SeeDealViewModel)DataContext).AccommodationsForTrip)
            {
                Pushpin accommodationPushpin = mapService.CreatePushpin(
                    accommodation.Location.Latitude,
                    accommodation.Location.Longitude,
                    accommodation.Name,
                    $"Restoraunt_{accommodation.Id}",
                    $"{consts.PathToIcons}/{consts.AccommodationPushpin}");
                mapControl.Children.Add(accommodationPushpin);
            }
        }

        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            DrawPointsOfInterestPushpins();

            MapService mapService = ((SeeDealViewModel)DataContext).MapService;

            TripModel trip = ((SeeDealViewModel)DataContext).Trip;

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

    }
}
