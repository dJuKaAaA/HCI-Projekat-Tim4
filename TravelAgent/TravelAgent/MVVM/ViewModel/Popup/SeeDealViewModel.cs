using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel.Popup
{
    public class SeeDealViewModel : Core.ViewModel
    {
        private TripModel _trip;

        public TripModel Trip
        {
            get { return _trip; }
            set { _trip = value; OnPropertyChanged(); }
        }

        private int _tripDuration;

        public int TripDuration
        {
            get { return _tripDuration; }
            set { _tripDuration = value; OnPropertyChanged(); }
        }

        public ICommand CloseCommand { get; set; }

        public SeeDealViewModel()
        {
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<SeeDealPopup>().SingleOrDefault(w => w.IsActive);

            if (currentWindow != null)
            {
                currentWindow.Close();
            }
        }
    }
}
