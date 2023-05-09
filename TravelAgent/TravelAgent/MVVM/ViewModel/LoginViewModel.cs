using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using TravelAgent.Core;
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

		public ICommand LoginCommand { get; }

        public LoginViewModel(
			UserService userService)
        {
			_userService = userService;

			LoginCommand = new RelayCommand(OnLogin, CanLogin);
        }

		private bool CanLogin(object o)
		{
			return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
		}

		private async void OnLogin(object o)
		{
			bool foundUser = await _userService.Login(Username, Password);
			if (foundUser)
			{
				MessageBox.Show("Login successful!");
			}
			else
			{
				MessageBox.Show("Invalid credentials!");
				Password = string.Empty;
				return;
			}
		}

    }
}
