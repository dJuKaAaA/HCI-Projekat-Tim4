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
        private CreateTripViewModel _viewModel;

        public CreateTripView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = (CreateTripViewModel)DataContext;
            _viewModel.DepartureAddressSearched += OnDepartureAddressSearched;
            _viewModel.DestinationAddressSearched += OnDestinationAddressSearched;
            DrawPointsOfInterestPushpins();
            if (_viewModel.Modifying)
            {
                // setting up departure attributes
                _departurePushpin = _viewModel.MapService.CreatePushpin(
                    _viewModel.TripForModification.Departure.Latitude,
                    _viewModel.TripForModification.Departure.Longitude,
                    _viewModel.TripForModification.Departure.Address,
                    "Departure");
                _viewModel.SelectedDepartureLocation = _viewModel.TripForModification.Departure;
                _viewModel.DepartureAddress = _viewModel.TripForModification.Departure.Address;

                // settings up destination attributes
                _destinationPushpin = _viewModel.MapService.CreatePushpin(
                    _viewModel.TripForModification.Destination.Latitude,
                    _viewModel.TripForModification.Destination.Longitude,
                    _viewModel.TripForModification.Destination.Address,
                    "Destination");
                _viewModel.SelectedDestinationLocation = _viewModel.TripForModification.Destination;
                _viewModel.DestinationAddress = _viewModel.TripForModification.Destination.Address;

                // creating the line that connects departure and destination
                _tripLine = _viewModel.MapService.CreatePushpinLine(_departurePushpin.Location, _destinationPushpin.Location);

                mapControl.Children.Add(_departurePushpin);
                mapControl.Children.Add(_destinationPushpin);
                mapControl.Children.Add(_tripLine);
                
            }
        }

        private void OnDestinationAddressSearched(object? sender, LocationModel location)
        {
            Service.MapService mapService = _viewModel.MapService;

            if (_departurePushpin == null)
            {
                _destinationPushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Destination");
                mapControl.Children.Add(_destinationPushpin);
                _viewModel.SelectedDestinationLocation = location;
                _viewModel.DestinationAddress = location.Address;
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
                _viewModel.SelectedDestinationLocation = location;
                _viewModel.DestinationAddress = location.Address;

                _tripLine = mapService.CreatePushpinLine(_departurePushpin.Location, _destinationPushpin.Location);
                mapControl.Children.Add(_tripLine);
            }

            mapControl.Center = new Location(location.Latitude, location.Longitude);
            mapControl.ZoomLevel = 12;
        }

        private async void DrawPointsOfInterestPushpins()
        {
            await DrawAccomodationsPushpins();
            await DrawRestorauntsPushpins();
            await DrawTouristAttractionPushpins();
        }

        private async Task DrawRestorauntsPushpins()
        {
            Service.MapService mapService = _viewModel.MapService;
            Consts consts = _viewModel.Consts;

            await _viewModel.LoadAllRestoraunts();

            foreach (RestorauntModel restoraunt in _viewModel.AllRestoraunts)
            {
                Pushpin pushpin = mapService.CreatePushpin(
                    restoraunt.Location.Latitude,
                    restoraunt.Location.Longitude,
                    restoraunt.Name,
                    $"Restoraunt_{restoraunt.Id}",
                    $"{consts.PathToIcons}/{consts.RestorauntPushpinIcon}");
                mapControl.Children.Add(pushpin);
            }

        }

        private async Task DrawAccomodationsPushpins()
        {
            Service.MapService mapService = _viewModel.MapService;
            Consts consts = _viewModel.Consts;

            await _viewModel.LoadAllAccommodations();

            foreach (AccommodationModel accommodation in _viewModel.AllAccommodations)
            {
                Pushpin pushpin = mapService.CreatePushpin(
                    accommodation.Location.Latitude,
                    accommodation.Location.Longitude,
                    accommodation.Name,
                    $"Accommodation_{accommodation.Id}",
                    $"{consts.PathToIcons}/{consts.AccommodationPushpinIcon}");
                mapControl.Children.Add(pushpin);
            }

        }

        private async Task DrawTouristAttractionPushpins()
        {
            Service.MapService mapService = _viewModel.MapService;
            Consts consts = _viewModel.Consts;

            await _viewModel.LoadAllTouristAttractions();

            foreach (TouristAttractionModel touristAttraction in _viewModel.AllTouristAttractions)
            {
                Pushpin pushpin = mapService.CreatePushpin(
                    touristAttraction.Location.Latitude,
                    touristAttraction.Location.Longitude,
                    touristAttraction.Name,
                    $"TouristAttraction_{touristAttraction.Id}",
                    $"{consts.PathToIcons}/{consts.TouristAttractionPushpinIcon}");
                mapControl.Children.Add(pushpin);
            }

        }

        private void OnDepartureAddressSearched(object? sender, LocationModel location)
        {
            Service.MapService mapService = _viewModel.MapService;

            if (_destinationPushpin == null)
            {
                _departurePushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Departure");
                mapControl.Children.Add(_departurePushpin);
                _viewModel.SelectedDepartureLocation = location;
                _viewModel.DepartureAddress = location.Address;
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
                _viewModel.SelectedDepartureLocation = location;
                _viewModel.DepartureAddress = location.Address;

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
            Service.MapService mapService = _viewModel.MapService;

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
                _viewModel.SelectedDepartureLocation = location;
                _viewModel.DepartureAddress = location.Address;
            }
            else if (_destinationPushpin == null)
            {
                _destinationPushpin = mapService.CreatePushpin(
                    location.Latitude,
                    location.Longitude,
                    location.Address,
                    "Destination");
                mapControl.Children.Add(_destinationPushpin);
                _viewModel.SelectedDestinationLocation = location;
                _viewModel.DestinationAddress = location.Address;

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
                _viewModel.SelectedDepartureLocation = location;
                _viewModel.SelectedDestinationLocation = null;

                _viewModel.DepartureAddress = location.Address;
                _viewModel.DestinationAddress = string.Empty;

                mapControl.Children.Add(_departurePushpin);

            }
        }

        // drag and drop
        //---
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        // restoraunts
        Point restorauntStartPoint = new Point();

        private void RestorauntListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            restorauntStartPoint = e.GetPosition(null);
        }

        private void RestorauntListView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = restorauntStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem =
                    FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                RestorauntModel restoraunt = (RestorauntModel)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", restoraunt);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private void RestorauntListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void RestorauntListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                RestorauntModel restoraunt = e.Data.GetData("myFormat") as RestorauntModel;
                if (!_viewModel.RestorauntsForTrip.Contains(restoraunt))
                {
                    _viewModel.RestorauntsForTrip.Add(restoraunt);
                }
            }
        }

        // accommodations
        Point accommodationStartPoint = new Point();

        private void AccommodationListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            accommodationStartPoint = e.GetPosition(null);
        }

        private void AccommodationListView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = accommodationStartPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem =
                    FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                AccommodationModel accommodation = (AccommodationModel)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", accommodation);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private void AccommodationsStartPointListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void AccommodationListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                AccommodationModel accommodation = e.Data.GetData("myFormat") as AccommodationModel;
                if (!_viewModel.AccommodationsForTrip.Contains(accommodation))
                {
                    _viewModel.AccommodationsForTrip.Add(accommodation);
                }
            }
        }

        // tourist attractions
        Point touristAttractionStartingPoint = new Point();

        private void TouristAttractionListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            touristAttractionStartingPoint = e.GetPosition(null);
        }

        private void TouristAttractionListView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = touristAttractionStartingPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem =
                    FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                TouristAttractionModel touristAttraction = (TouristAttractionModel)listView.ItemContainerGenerator.
                    ItemFromContainer(listViewItem);

                // Initialize the drag & drop operation
                DataObject dragData = new DataObject("myFormat", touristAttraction);
                DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
            }
        }

        private void TouristAttractionListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TouristAttractionListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                TouristAttractionModel touristAttraction = e.Data.GetData("myFormat") as TouristAttractionModel;
                if (!_viewModel.TouristAttractionsForTrip.Contains(touristAttraction))
                {
                    _viewModel.TouristAttractionsForTrip.Add(touristAttraction);
                }
            }
        }

    }
}
