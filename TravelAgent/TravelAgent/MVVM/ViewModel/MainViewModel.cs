using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class MainViewModel : Core.ViewModel
    {
        public NavigationService NavigationService { get; }

        public MainViewModel(
            NavigationService navigationService)
        {
            NavigationService = navigationService;

            NavigationService.NavigateTo<LoginViewModel>();
        }
    }
}
