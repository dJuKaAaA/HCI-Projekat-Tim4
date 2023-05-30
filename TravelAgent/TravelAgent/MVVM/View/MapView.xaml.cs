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
using TravelAgent.Service;

namespace TravelAgent.MVVM.View
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        private MapViewModel _viewModel;

        public MapView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = (MapViewModel)DataContext;
            DrawPointsOfInterest();
        }

        private async void DrawPointsOfInterest()
        {
            MapService mapService = _viewModel.MapService;
            Consts consts = _viewModel.Consts;

            // Draw tourist attractions
            await _viewModel.LoadTouristAttractions();
            foreach (TouristAttractionModel touristAttraction in _viewModel.AllTouristAttractions)
            {
                Pushpin touristAttractionPushpin = mapService.CreatePushpin(
                    touristAttraction.Location.Latitude,
                    touristAttraction.Location.Longitude,
                    touristAttraction.Name,
                    $"TouristAttraction_{touristAttraction.Id}",
                    $"{consts.PathToIcons}/{consts.TouristAttractionPushpinIcon}");
                mapControl.Children.Add(touristAttractionPushpin);
            }

            // Draw restaurants
            await _viewModel.LoadRestaurants();
            foreach (RestaurantModel restaurant in _viewModel.AllRestaurants)
            {
                Pushpin restaurantPushpin = mapService.CreatePushpin(
                    restaurant.Location.Latitude,
                    restaurant.Location.Longitude,
                    restaurant.Name,
                    $"Restaurant_{restaurant.Id}",
                    $"{consts.PathToIcons}/{consts.RestaurantPushpinIcon}");
                mapControl.Children.Add(restaurantPushpin);
            }

            // Draw accommodations
            await _viewModel.LoadAccommodations();
            foreach (AccommodationModel accommodation in _viewModel.AllAccommodations)
            {
                Pushpin accommodationPushpin = mapService.CreatePushpin(
                    accommodation.Location.Latitude,
                    accommodation.Location.Longitude,
                    accommodation.Name,
                    $"Accommodation_{accommodation.Id}",
                    $"{consts.PathToIcons}/{consts.AccommodationPushpinIcon}");
                mapControl.Children.Add(accommodationPushpin);
            }
        }

    }
}
