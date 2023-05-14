using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class MainViewModel : Core.ViewModel
    {
        private Visibility _menuVisibility;

        public Visibility MenuVisibility
        {
            get { return _menuVisibility; }
            set { _menuVisibility = value; OnPropertyChanged(); }
        }

        public static UserModel? SignedUser { get; set; }

        public NavigationService NavigationService { get; }

        public ICommand OpenAllFlightsViewCommand { get; }
        public ICommand OpenAllTouristAttractionsViewCommand { get; }
        public ICommand OpenAllRestorauntsViewCommand { get; }
        public ICommand OpenAllAccomodationsViewCommand { get; }
        public ICommand OpenPurchasedFlightsViewCommand { get; }
        public ICommand OpenMapsCommand { get; }
        public ICommand OpenHelpCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(
            NavigationService navigationService)
        {
            NavigationService = navigationService;

            OpenAllFlightsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllFlightsViewModel>(), o => true);
            OpenAllTouristAttractionsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllTouristAttractionsViewModel>(), o => true);
            OpenAllRestorauntsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllRestorauntsViewModel>(), o => true);
            OpenAllAccomodationsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllAccomodationsViewModel>(), o => true);
            OpenPurchasedFlightsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<PurchasedFlightsViewModel>(), o => true);
            OpenMapsCommand = new RelayCommand(o => NavigationService.NavigateTo<MapViewModel>(), o => true);
            OpenHelpCommand = new RelayCommand(o => MessageBox.Show("This is very helpful :)"), o => true);
            LogoutCommand = new RelayCommand(OnLogout, o => true);

            NavigationService.NavigationCompleted += (object? sender, Type viewModelType) =>
            {
                if (viewModelType == typeof(LoginViewModel) || viewModelType == typeof(RegisterViewModel))
                {
                    MenuVisibility = Visibility.Collapsed;
                }
                else
                {
                    MenuVisibility = Visibility.Visible;
                }
            };

            NavigationService.NavigateTo<AllFlightsViewModel>();
        }

        private void OnLogout(object o)
        {
            SignedUser = null;
            NavigationService.NavigateTo<LoginViewModel>();
        }
    }

}
