using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel
{
    public class CreateTripViewModel : Core.ViewModel
    {
        private CreateLocationPopup? _createLocationPopup;

        public ICommand OpenCreateLocationPopupCommand { get; }

        public CreateTripViewModel()
        {

            OpenCreateLocationPopupCommand = new Core.RelayCommand(OnOpenCreateLocationPopup, o => true);
        }

        private void OnOpenCreateLocationPopup(object o)
        {
            _createLocationPopup?.Close();
            _createLocationPopup = new CreateLocationPopup();
            _createLocationPopup.Show();
        }
    }
}
