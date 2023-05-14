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
            MapDataContext.LoadedFlights += OnLoadedFlights;
        }

        private void OnLoadedFlights(object? sender, EventArgs e)
        {
            foreach (FlightModel flight in MapDataContext.AllFlights)
            {
                MakeTakeoffPin(flight.Departure.Latitude, flight.Departure.Longitude);
                MakeLandingPin(flight.Destination.Latitude, flight.Destination.Longitude);
                MakeFlightLine(flight.Departure, flight.Destination);
            }
        }

        private void MakeTakeoffPin(double latitude, double longitude)
        {
            MakePin(latitude, longitude, $"{MapDataContext.Consts.PathToIcons}/airplanetakeoff.png");
        }

        private void MakeLandingPin(double latitude, double longitude)
        {
            MakePin(latitude, longitude, $"{MapDataContext.Consts.PathToIcons}/airplanelanding.png");
        }

        private void MakePin(double latitude, double longitude, string imagePath)
        {
            Pushpin pushpin = new Pushpin();
            pushpin.Location = new Location(latitude, longitude);
            pushpin.Width = 50;
            pushpin.Height = 50;

            // Create a ControlTemplate for the Pushpin
            ControlTemplate controlTemplate = new ControlTemplate(typeof(Pushpin));

            // Create an Image element and set its properties
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));

            // Create an instance of FrameworkElementFactory to create the visual tree of the ControlTemplate
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Image));
            factory.SetValue(Image.SourceProperty, image.Source);
            factory.SetValue(Image.WidthProperty, image.Width);
            factory.SetValue(Image.HeightProperty, image.Height);

            // Set the visual tree of the ControlTemplate to the FrameworkElementFactory
            controlTemplate.VisualTree = factory;

            // Set the ControlTemplate of the Pushpin
            pushpin.Template = controlTemplate;

            // Add the Pushpin to the Map
            mapControl.Children.Add(pushpin);
        }

        private void MakeFlightLine(LocationModel departure, LocationModel destination)
        {
            // Create a new MapPolyline
            MapPolyline mapPolyline = new MapPolyline();
            mapPolyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 17, 17, 17));
            mapPolyline.StrokeThickness = 2;
            mapPolyline.StrokeDashArray = new System.Windows.Media.DoubleCollection() { 4 };

            // Create a new LocationCollection
            LocationCollection locationCollection = new LocationCollection();
            locationCollection.Add(new Location(departure.Latitude, departure.Longitude));
            locationCollection.Add(new Location(destination.Latitude, destination.Longitude));

            // Set the Locations property of the MapPolyline
            mapPolyline.Locations = locationCollection;

            // Add the MapPolyline to the Children collection of the Map
            mapControl.Children.Add(mapPolyline);
        }

    }
}
