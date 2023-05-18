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
        private Visibility _travelerMenuVisibility;

        public Visibility TravelerMenuVisibility
        {
            get { return _travelerMenuVisibility; }
            set { _travelerMenuVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _agentMenuVisibility;

        public Visibility AgentMenuVisibility
        {
            get { return _agentMenuVisibility; }
            set { _agentMenuVisibility = value; OnPropertyChanged(); }
        }

        public static UserModel? SignedUser { get; set; }
        public static UserType? SignedUserType => SignedUser?.Type;

        public NavigationService NavigationService { get; }

        public ICommand OpenAllTripsViewCommand { get; }
        public ICommand OpenAllTouristAttractionsViewCommand { get; }
        public ICommand OpenAllRestorauntsViewCommand { get; }
        public ICommand OpenAllAccomodationsViewCommand { get; }
        public ICommand OpenUserTripsViewCommand { get; }
        public ICommand OpenMapsCommand { get; }
        public ICommand OpenHelpCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(
            NavigationService navigationService)
        {
            NavigationService = navigationService;

            OpenAllTripsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllTripsViewModel>(), o => true);
            OpenAllTouristAttractionsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllTouristAttractionsViewModel>(), o => true);
            OpenAllRestorauntsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllRestorauntsViewModel>(), o => true);
            OpenAllAccomodationsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllAccommodationsViewModel>(), o => true);
            OpenUserTripsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<UserTripsViewModel>(), o => true);
            OpenMapsCommand = new RelayCommand(o => NavigationService.NavigateTo<MapViewModel>(), o => true);
            OpenHelpCommand = new RelayCommand(o => MessageBox.Show("This is very helpful :)"), o => true);
            LogoutCommand = new RelayCommand(OnLogout, o => true);

            NavigationService.NavigationCompleted += (object? sender, Type viewModelType) =>
            {
                if (viewModelType == typeof(LoginViewModel) || viewModelType == typeof(RegisterViewModel))
                {
                    TravelerMenuVisibility = Visibility.Collapsed;
                    AgentMenuVisibility = Visibility.Collapsed;
                }
                else
                {
                    if (SignedUser?.Type == UserType.Traveler)
                    {
                        TravelerMenuVisibility = Visibility.Visible;
                        AgentMenuVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TravelerMenuVisibility = Visibility.Collapsed;
                        AgentMenuVisibility = Visibility.Visible;
                    }
                }
            };

            NavigationService.NavigateTo<LoginViewModel>();
        }

        private void OnLogout(object o)
        {
            SignedUser = null;
            NavigationService.NavigateTo<LoginViewModel>();
        }
    }

}
