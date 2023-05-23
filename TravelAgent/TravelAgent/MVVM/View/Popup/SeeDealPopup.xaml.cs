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
            TouristAttractionService touristAttractionService,
            MapService mapService)
        {
            InitializeComponent();

            ((SeeDealViewModel)DataContext).Trip = trip;
            ((SeeDealViewModel)DataContext).UserTripService = userTripService;
            ((SeeDealViewModel)DataContext).TouristAttractionService = touristAttractionService;
            ((SeeDealViewModel)DataContext).MapService = mapService;
            DateTime takeoff = ((SeeDealViewModel)DataContext).Trip.DepartureDateTime;
            DateTime landing = ((SeeDealViewModel)DataContext).Trip.ArrivalDateTime;
            TimeSpan timeDiff = landing - takeoff;
            ((SeeDealViewModel)DataContext).TripDuration = timeDiff.Hours;
        }

        private async void DrawPointsOfInterestPushpins()
        {
            MapService mapService = ((SeeDealViewModel)DataContext).MapService;
            // TODO: Add special icons for each type of point of interest

            // drawing tourist attractions
            await ((SeeDealViewModel)DataContext).LoadTouristAttractionsForTrip();
            foreach (TouristAttractionModel touristAttraction in ((SeeDealViewModel)DataContext).TouristAttractionsForTrip)
            {
                Pushpin touristAttractionPushpin = mapService.CreatePushpin(
                    touristAttraction.Location.Latitude,
                    touristAttraction.Location.Longitude,
                    touristAttraction.Name,
                    $"TouristAttraction_{touristAttraction.Id}");
                mapControl.Children.Add(touristAttractionPushpin);
            }

            // TODO: Draw restoraunts

            // TODO: Draw accommodations
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
