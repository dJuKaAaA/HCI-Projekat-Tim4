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

namespace TravelAgent.MVVM.View.Popup
{
    public partial class PickLocationPopup : Window
    {
        private Core.CreationViewModel _viewModel;

        private Pushpin _pushpin;

        public PickLocationPopup()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = (Core.CreationViewModel)DataContext;
            _viewModel.AddressSearched += OnAddressSearched;
        }

        private void OnAddressSearched(object? sender, LocationModel location)
        {
            DrawPushpin(location);
            _viewModel.Address = _viewModel.Location.Address;
            mapControl.Center = new Location(_viewModel.Location.Latitude, _viewModel.Location.Longitude);
            mapControl.ZoomLevel = 12;
        }

        private void DrawPushpin(LocationModel location)
        {
            if (_pushpin != null)
            {
                mapControl.Children.Remove(_pushpin);
            }

            _pushpin = _viewModel.MapService.CreatePushpin(
                location.Latitude,
                location.Longitude,
                location.Address,
                "PickedLocation");

            mapControl.Children.Add(_pushpin);
        }

        private async void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Get the clicked location from the event arguments
            Point mousePosition = e.GetPosition((UIElement)sender);
            Location clickedLocation = mapControl.ViewportPointToLocation(mousePosition);

            _viewModel.Location = await _viewModel.MapService.ReverseGeocode(clickedLocation.Latitude, clickedLocation.Longitude);

            _viewModel.Address = _viewModel.Location.Address;

            DrawPushpin(_viewModel.Location);
        }

    }
}
