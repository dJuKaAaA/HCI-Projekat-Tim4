using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.MVVM.View.Popup;

namespace TravelAgent.MVVM.ViewModel.Popup
{
    public class CreateLocationViewModel : Core.ViewModel
    {

        public ICommand CloseCommand { get; }

        public CreateLocationViewModel()
        {
            CloseCommand = new RelayCommand(OnClose, o => true);
        }

        private void OnClose(object o)
        {
            Window currentWindow = Application.Current.Windows.OfType<CreateLocationPopup>().SingleOrDefault(w => w.IsActive);

            currentWindow?.Close();
        }
    }
}
