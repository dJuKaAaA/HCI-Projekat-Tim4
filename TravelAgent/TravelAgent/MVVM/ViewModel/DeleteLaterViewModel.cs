using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TravelAgent.MVVM.ViewModel
{
    public class DeleteLaterViewModel : Core.ViewModel
    {
        private readonly Service.NavigationService _navigationService;

        public ICommand GoToHomeCommand { get; }

        public DeleteLaterViewModel(
            Service.NavigationService navigationService)
        {
            _navigationService = navigationService;

            GoToHomeCommand = new Core.RelayCommand((object o) => _navigationService.NavigateTo<HomeViewModel>(), (object o) => true);
        }
    }
}
