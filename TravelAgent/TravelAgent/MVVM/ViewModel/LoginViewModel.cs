using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using TravelAgent.Core;
using TravelAgent.Exception;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class LoginViewModel : Core.ViewModel
    {
		private string _username;

		public string Username
		{
			get { return _username; }
			set { _username = value; OnPropertyChanged(); }
		}

		private string _password;

		public string Password
		{
			get { return _password; }
			set { _password = value; OnPropertyChanged(); }
		}

		private readonly UserService _userService;
		private readonly NavigationService _navigationService;

		private bool _loginCommandRunning = false;

		public ICommand LoginCommand { get; }
		public ICommand NavigateToRegisterViewCommand { get; }

        public LoginViewModel(
			UserService userService,
			NavigationService navigationService)
        {
			_userService = userService;
			_navigationService = navigationService;

			LoginCommand = new RelayCommand(OnLogin, CanLogin);
			NavigateToRegisterViewCommand = new RelayCommand(OnNavigateToRegisterView, o => true);
        }

		private bool CanLogin(object o)
		{
			return !string.IsNullOrWhiteSpace(Username) && 
				!string.IsNullOrWhiteSpace(Password) &&
				!_loginCommandRunning;
		}

		private async void OnLogin(object o)
		{
			_loginCommandRunning = true;

			try
			{
                MainViewModel.SignedUser = await _userService.Login(Username, Password);
                _navigationService.NavigateTo<AllTripsViewModel>();
            }
            catch (DatabaseResponseException e)
			{
				MessageBox.Show(e.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
				Password = string.Empty;
				return;
			}
			finally
			{
				_loginCommandRunning = false;
			}
		}

		private void OnNavigateToRegisterView(object o)
		{
			_navigationService.NavigateTo<RegisterViewModel>();
		}

    }
}
