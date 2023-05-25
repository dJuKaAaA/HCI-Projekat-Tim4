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

            // Draw restoraunts
            await _viewModel.LoadRestoraunts();
            foreach (RestorauntModel restoraunt in _viewModel.AllRestoraunts)
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
            await _viewModel.LoadAccommodations();
            foreach (AccommodationModel accommodation in _viewModel.AllAccommodations)
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
