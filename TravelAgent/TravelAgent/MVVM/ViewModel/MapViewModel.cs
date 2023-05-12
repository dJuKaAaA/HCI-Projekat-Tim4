using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.MVVM.Model;

namespace TravelAgent.MVVM.ViewModel
{
    public class MapViewModel : Core.ViewModel
    {
        public Pushpin Pushpin { get; }

        public MapViewModel()
        {
            Pushpin = new Pushpin();
            Pushpin.Location = new Location(47.6097, -122.3331);  // Set the latitude and longitude of the pushpin
            Pushpin.ToolTip = "Marker 1";  // Optional tooltip for the pushpin
        }
    }
}
