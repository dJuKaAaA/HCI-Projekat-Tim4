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

        private Visibility _unregisteredUserMenuVisibility;

        public Visibility UnregisteredUserMenuVisibility
        {
            get { return _unregisteredUserMenuVisibility; }
            set { _unregisteredUserMenuVisibility = value; OnPropertyChanged(); }
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
        public ICommand OpenLoginViewCommand { get; }
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
            OpenLoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo<LoginViewModel>(), o => true);
            LogoutCommand = new RelayCommand(OnLogout, o => true);

            NavigationService.NavigationCompleted += (object? sender, NavigationEventArgs e) =>
            {
                if (e.ViewModelType == typeof(LoginViewModel) || e.ViewModelType == typeof(RegisterViewModel))
                {
                    TravelerMenuVisibility = Visibility.Collapsed;
                    AgentMenuVisibility = Visibility.Collapsed;
                    UnregisteredUserMenuVisibility = Visibility.Visible;
                }
                else
                {
                    switch (SignedUser?.Type)
                    {
                        case UserType.Traveler:
                            TravelerMenuVisibility = Visibility.Visible;
                            AgentMenuVisibility = Visibility.Collapsed;
                            UnregisteredUserMenuVisibility = Visibility.Collapsed;
                            break;
                        case UserType.Agent:
                            TravelerMenuVisibility = Visibility.Collapsed;
                            AgentMenuVisibility = Visibility.Visible;
                            UnregisteredUserMenuVisibility = Visibility.Collapsed;
                            break;
                    }
                }
            };

            UnregisteredUserMenuVisibility = Visibility.Visible;
            AgentMenuVisibility = Visibility.Collapsed;
            TravelerMenuVisibility = Visibility.Collapsed;

            NavigationService.NavigateTo<LoginViewModel>();
        }

        private void OnLogout(object o)
        {
            SignedUser = null;
            NavigationService.NavigateTo<LoginViewModel>();
        }
    }

}
