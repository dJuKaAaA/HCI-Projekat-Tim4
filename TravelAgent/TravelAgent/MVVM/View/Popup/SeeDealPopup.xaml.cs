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
        public SeeDealPopup(TripModel trip, UserTripService userTripService)
        {
            InitializeComponent();

            ((SeeDealViewModel)DataContext).Trip = trip;
            ((SeeDealViewModel)DataContext).UserTripService = userTripService;
            DateTime takeoff = ((SeeDealViewModel)DataContext).Trip.DepartureDateTime;
            DateTime landing = ((SeeDealViewModel)DataContext).Trip.ArrivalDateTime;
            TimeSpan timeDiff = landing - takeoff;
            ((SeeDealViewModel)DataContext).TripDuration = timeDiff.Hours;
        }
    }
}
