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
using TravelAgent.MVVM.ViewModel;

namespace TravelAgent.MVVM.View
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : UserControl
    {
        public MapViewModel MapDataContext { get; } 

        public MapView()
        {
            InitializeComponent();
            MapDataContext = (MapViewModel)DataContext;

            // Add the pushpin to the map control
            mapControl.Children.Add(MapDataContext.Pushpin);
        }
    }
}
