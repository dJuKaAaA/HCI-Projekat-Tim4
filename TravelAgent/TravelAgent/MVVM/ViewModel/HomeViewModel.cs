using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TravelAgent.MVVM.ViewModel
{
    public class HomeViewModel : Core.ViewModel
    {
        private readonly Service.NavigationService _navigationService;

        public ICommand LogoutCommand { get; }

        public HomeViewModel(
            Service.NavigationService navigationService)
        {
            _navigationService = navigationService;

            LogoutCommand = new Core.RelayCommand((object o) => _navigationService.NavigateTo<LoginViewModel>(), (object o) => true);
        }
    }
}
