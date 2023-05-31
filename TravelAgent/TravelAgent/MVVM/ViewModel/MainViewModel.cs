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

        private static KeyBinding? _openCreateViewKeyBinding;
        private static KeyBinding? _openModifyViewKeyBinding;
        private static KeyBinding? _deleteEntityKeyBinding;
        private static KeyBinding? _openSearchKeyBinding;

        public NavigationService NavigationService { get; }

        public ICommand OpenAllTripsViewCommand { get; }
        public ICommand OpenAllTouristAttractionsViewCommand { get; }
        public ICommand OpenAllRestaurantsViewCommand { get; }
        public ICommand OpenAllAccomodationsViewCommand { get; }
        public ICommand OpenUserTripsViewCommand { get; }
        public ICommand OpenSoldTripsViewCommand { get; }
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
            OpenAllRestaurantsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllRestaurantsViewModel>(), o => true);
            OpenAllAccomodationsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<AllAccommodationsViewModel>(), o => true);
            OpenUserTripsViewCommand = new RelayCommand(o => NavigationService.NavigateTo<UserTripsViewModel>(), o => SignedUser != null);
            OpenMapsCommand = new RelayCommand(o => NavigationService.NavigateTo<MapViewModel>(), o => true);
            OpenHelpCommand = new RelayCommand(o => MessageBox.Show("This is very helpful :)"), o => SignedUser != null);
            OpenLoginViewCommand = new RelayCommand(o => NavigationService.NavigateTo<LoginViewModel>(), o => SignedUser == null);
            LogoutCommand = new RelayCommand(OnLogout, o => SignedUser != null);

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
            MessageBoxResult result = MessageBox.Show("Log out?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SignedUser = null;
                NavigationService.NavigateTo<LoginViewModel>();
            }
        }

        public static void AddOpenSearchKeyBinding(ICommand openSearchCommand)
        {
            _openSearchKeyBinding = new KeyBinding(openSearchCommand, Key.F, ModifierKeys.Control);
            Window window = Application.Current.MainWindow;
            window.InputBindings.Add(_openSearchKeyBinding);
        }

        public static void RemoveOpenSearchKeyBinding()
        {
            Window window = Application.Current.MainWindow;
            window.InputBindings.Remove(_openSearchKeyBinding);
            _openSearchKeyBinding = null;
        }

        public static void AddCUDKeyBindings(
            ICommand openCreateViewCommand,
            ICommand openModifyViewCommand,
            ICommand deleteEntityCommand)
        {
            Window window = Application.Current.MainWindow;
            _openCreateViewKeyBinding = new KeyBinding(openCreateViewCommand, Key.N, ModifierKeys.Control);
            _openModifyViewKeyBinding = new KeyBinding(openModifyViewCommand, Key.C, ModifierKeys.Control);
            _deleteEntityKeyBinding = new KeyBinding(deleteEntityCommand, Key.D, ModifierKeys.Control);
            window.InputBindings.Add(_openCreateViewKeyBinding);
            window.InputBindings.Add(_openModifyViewKeyBinding);
            window.InputBindings.Add(_deleteEntityKeyBinding);
        } 

        public static void RemoveCUDKeyBindings()
        {
            if (_openCreateViewKeyBinding != null &&
                _openModifyViewKeyBinding != null &&
                _deleteEntityKeyBinding != null)
            {
                Window window = Application.Current.MainWindow;
                window.InputBindings.Remove(_openCreateViewKeyBinding);
                window.InputBindings.Remove(_openModifyViewKeyBinding);
                window.InputBindings.Remove(_deleteEntityKeyBinding);
                _openCreateViewKeyBinding = null;
                _openModifyViewKeyBinding = null;
                _deleteEntityKeyBinding = null;
            }
        }
    }

}
