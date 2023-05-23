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
            TouristAttractionService touristAttractionService)
        {
            InitializeComponent();

            ((SeeDealViewModel)DataContext).Trip = trip;
            ((SeeDealViewModel)DataContext).UserTripService = userTripService;
            ((SeeDealViewModel)DataContext).TouristAttractionService = touristAttractionService;
            DateTime takeoff = ((SeeDealViewModel)DataContext).Trip.DepartureDateTime;
            DateTime landing = ((SeeDealViewModel)DataContext).Trip.ArrivalDateTime;
            TimeSpan timeDiff = landing - takeoff;
            ((SeeDealViewModel)DataContext).TripDuration = timeDiff.Hours;
        }

        private async void DrawPointsOfInterestPushpins()
        {
            // TODO: Add special icons for each type of point of interest

            // drawing tourist attractions
            await ((SeeDealViewModel)DataContext).LoadTouristAttractionsForTrip();
            foreach (TouristAttractionModel touristAttraction in ((SeeDealViewModel)DataContext).TouristAttractionsForTrip)
            {
                Pushpin touristAttractionPushpin = CreatePushpin(
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

            TripModel trip = ((SeeDealViewModel)DataContext).Trip;

            Pushpin departurePushpin = CreatePushpin(
                trip.Departure.Latitude, 
                trip.Departure.Longitude, 
                trip.Departure.Address, 
                $"Departure_{trip.Departure.Id}");
            Pushpin destinationPushpin = CreatePushpin(
                trip.Destination.Latitude, 
                trip.Destination.Longitude, 
                trip.Destination.Address, 
                $"Destination_{trip.Destination.Id}");

            MapPolyline line = CreatePushpinLine(departurePushpin.Location, destinationPushpin.Location);

            mapControl.Children.Add(departurePushpin);
            mapControl.Children.Add(destinationPushpin);
            mapControl.Children.Add(line);

            mapControl.Center = departurePushpin.Location;
        }

        private MapPolyline CreatePushpinLine(Location startLocation, Location endLocation)
        {
            // Create a MapPolyline object
            var polyline = new MapPolyline();

            // Set the stroke color and thickness
            polyline.Stroke = new SolidColorBrush(Colors.Red);
            polyline.StrokeThickness = 4;

            // Set the stroke dash array to create a dashed line effect
            var dashedArray = new DoubleCollection { 2 };
            polyline.StrokeDashArray = dashedArray;

            // Create a LocationCollection for the polyline's points
            var locations = new LocationCollection();
            locations.Add(startLocation);
            locations.Add(endLocation);

            // Set the polyline's locations
            polyline.Locations = locations;

            return polyline;
        }

        private Pushpin CreatePushpin(double latitude, double longitude, string toolTipText, string tag, string? imagePath = null)
        {
            // Create a new Pushpin
            Pushpin pushpin = new Pushpin();
            pushpin.Width = 50;
            pushpin.Height = 50;
            pushpin.ToolTip = toolTipText;
            pushpin.Tag = tag;

            //pushpin.MouseLeftButtonDown += Pushpin_Click;

            // Set the Location of the Pushpin
            pushpin.Location = new Location(latitude, longitude);

            // Create the custom style for the pushpin
            Style style = new Style(typeof(Pushpin));

            // Create the triggers for mouse enter and leave events
            Trigger mouseEnterTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
            mouseEnterTrigger.Setters.Add(new Setter(RenderTransformProperty, new ScaleTransform(1.2, 1.2)));
            mouseEnterTrigger.Setters.Add(new Setter(Panel.ZIndexProperty, 1));
            mouseEnterTrigger.Setters.Add(new Setter(Control.CursorProperty, Cursors.Hand));

            Trigger mouseLeaveTrigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = false };
            mouseLeaveTrigger.Setters.Add(new Setter(RenderTransformProperty, new ScaleTransform(1, 1)));
            mouseLeaveTrigger.Setters.Add(new Setter(Panel.ZIndexProperty, 0));

            // Add the triggers to the style
            style.Triggers.Add(mouseEnterTrigger);
            style.Triggers.Add(mouseLeaveTrigger);

            // Apply the style to the pushpin
            pushpin.Style = style;

            return pushpin;
        }

    }
}
