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

namespace TravelAgent.MVVM.View.Popup
{
    /// <summary>
    /// Interaction logic for SeeDealPopup.xaml
    /// </summary>
    public partial class SeeDealPopup : Window
    {
        public SeeDealPopup(FlightModel flight)
        {
            InitializeComponent();

            ((SeeDealViewModel)DataContext).Flight = flight;
            DateTime takeoff = ((SeeDealViewModel)DataContext).Flight.TakeoffDateTime;
            DateTime landing = ((SeeDealViewModel)DataContext).Flight.LandingDateTime;
            TimeSpan timeDiff = landing - takeoff;
            ((SeeDealViewModel)DataContext).FlightDuration = timeDiff.Hours;
        }
    }
}
