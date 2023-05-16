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
            foreach (LocationModel location in MapDataContext.AllLocations)
            {
                mapControl.Children.Add(CreatePushpin(location.Latitude, location.Longitude, location.Name, location.Id));
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

        private Pushpin CreatePushpin(double latitude, double longitude, string label, int tag)
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

        private void Pushpin_Click(object sender, MouseButtonEventArgs e)
        {
            Pushpin clickedPushpin = (Pushpin)sender;

            LocationModel location = MapDataContext.AllLocations.FirstOrDefault(l => l.Id == int.Parse(clickedPushpin.Tag.ToString()));

            // Set the image and text in the popup
            locationImage.Source = new BitmapImage(new Uri($"{MapDataContext.Consts.PathToLocationImages}/{location.Image}", UriKind.RelativeOrAbsolute));
            locationName.Text = location.Name;
            locationContainer.Visibility = Visibility.Visible;
        }
    }
}
