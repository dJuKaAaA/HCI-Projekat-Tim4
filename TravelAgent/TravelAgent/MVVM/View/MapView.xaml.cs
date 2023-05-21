using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

namespace TravelAgent.MVVM.View
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapViewModel MapDataContext => (MapViewModel)DataContext;

        public MapView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MapDataContext.LoadFinished += OnLoadFinished;
        }

        private void OnLoadFinished(object? sender, EventArgs e)
        {
            //foreach (LocationModel location in MapDataContext.AllLocations)
            //{
            //    mapControl.Children.Add(CreatePushpin(location.Latitude, location.Longitude, location.Name, location.Id));
            //}
            foreach (TripModel trip in MapDataContext.AllTrips)
            {
                Pushpin startPushpin = CreatePushpin(trip.Departure.Latitude, trip.Departure.Longitude, trip.Departure.Name, $"Departure_{trip.Departure.Id}");
                Pushpin endPushpin = CreatePushpin(trip.Destination.Latitude, trip.Destination.Longitude, trip.Destination.Name, $"Destination_{trip.Destination.Id}");

                mapControl.Children.Add(startPushpin);
                mapControl.Children.Add(endPushpin);
                DrawLine(startPushpin.Location, endPushpin.Location);
            }
        }

        private Pushpin CreatePushpin(double latitude, double longitude, string label, string tag)
        {
            // Create a new Pushpin
            Pushpin pushpin = new Pushpin();
            pushpin.Width = 50;
            pushpin.Height = 50;
            pushpin.ToolTip = label;
            pushpin.Tag = tag;

            pushpin.MouseLeftButtonDown += Pushpin_Click;

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

        private void DrawLine(Location startLocation, Location endLocation)
        {
            // Create a MapPolyline object
            var polyline = new MapPolyline();

            // Set the stroke color and thickness
            polyline.Stroke = new SolidColorBrush(Colors.Black);
            polyline.StrokeThickness = 2;

            // Set the stroke dash array to create a dashed line effect
            var dashedArray = new DoubleCollection { 2, 2 };
            polyline.StrokeDashArray = dashedArray;

            // Create a LocationCollection for the polyline's points
            var locations = new LocationCollection();
            locations.Add(startLocation);
            locations.Add(endLocation);

            // Set the polyline's locations
            polyline.Locations = locations;

            // Add the polyline to the map
            mapControl.Children.Add(polyline);
        }

        private void Pushpin_Click(object sender, MouseButtonEventArgs e)
        {
            Pushpin clickedPushpin = (Pushpin)sender;

            int locationId = int.Parse(clickedPushpin.Tag.ToString().Split("_")[1]);
            string locationPlaceValue = clickedPushpin.Tag.ToString().Split("_")[0];  // indicates whether the location is the departure or destination
            LocationModel? location = MapDataContext.AllLocations.FirstOrDefault(l => l.Id == locationId);

            if (location == null)
            {
                return;
            }

            // Set the image and text in the popup
            locationImage.Source = new BitmapImage(new Uri($"{MapDataContext.Consts.PathToLocationImages}/{location.Image}", UriKind.RelativeOrAbsolute));
            locationName.Text = location.Name;
            locationPlace.Text = locationPlaceValue;
            locationContainer.Visibility = Visibility.Visible;
        }

        private void closePopupButton_Click(object sender, RoutedEventArgs e)
        {
            locationContainer.Visibility = Visibility.Collapsed;
        }

        // !! CODE BELOW DOES NOT WORK !!
        // dragging the ContentControl which shows the location information
        //------------------------------------------------------------------------------------
        private bool isDragging;
        private Point dragStartPosition;
        private double originalLeft;
        private double originalTop;

        private void ContentControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var contentControl = (ContentControl)sender;

            // Capture the mouse and store the initial position
            contentControl.CaptureMouse();
            isDragging = true;
            dragStartPosition = e.GetPosition(contentControl);
            originalLeft = Canvas.GetLeft(contentControl);
            originalTop = Canvas.GetTop(contentControl);
        }

        private void ContentControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var contentControl = (ContentControl)sender;

                // Calculate the new position based on the mouse movement
                var currentPosition = e.GetPosition(contentControl);
                var deltaX = currentPosition.X - dragStartPosition.X;
                var deltaY = currentPosition.Y - dragStartPosition.Y;

                // Update the position of the ContentControl within the Canvas
                Canvas.SetLeft(contentControl, originalLeft + deltaX);
                Canvas.SetTop(contentControl, originalTop + deltaY);
            }
        }

        private void ContentControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var contentControl = (ContentControl)sender;

            // Release the mouse capture and stop dragging
            contentControl.ReleaseMouseCapture();
            isDragging = false;
        }
        //------------------------------------------------------------------------------------

    }
}
