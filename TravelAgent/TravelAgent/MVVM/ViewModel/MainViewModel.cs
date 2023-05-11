using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
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

        public NavigationService NavigationService { get; }

        public ICommand OpenHelpCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(
            NavigationService navigationService)
        {
            NavigationService = navigationService;

            OpenHelpCommand = new RelayCommand(o => MessageBox.Show("Treba pomoc aaaa pickoooo"), o => true);
            LogoutCommand = new RelayCommand(o => NavigationService.NavigateTo<LoginViewModel>(), o => true);

            NavigationService.NavigationCompleted += (object sender, Type viewModelType) =>
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
    }

}
