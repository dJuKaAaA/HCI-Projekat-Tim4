using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TravelAgent.Core;
using TravelAgent.Exception;
using TravelAgent.MVVM.Model;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class RegisterViewModel : Core.ViewModel
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private string _surname;

        public string Surname
        {
            get { return _surname; }
            set { _surname = value; OnPropertyChanged(); }
        }

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

        private string _confirmPassword;

        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        private readonly Service.NavigationService _navigateService;
        private readonly UserService _userService;

        public ICommand NavigateToLoginViewCommand { get; }
        public ICommand RegisterCommand { get; }

        public RegisterViewModel(
            Service.NavigationService navigationService,
            UserService userService)
        {
            _navigateService = navigationService;
            _userService = userService;

            NavigateToLoginViewCommand = new RelayCommand(OnNavigateToLoginView, CanNavigateToLoginView);
            RegisterCommand = new RelayCommand(OnRegister, CanRegister);
        }

        private bool CanNavigateToLoginView(object o)
        {
            return true;
        }

        private void OnNavigateToLoginView(object o)
        {
            _navigateService.NavigateTo<LoginViewModel>();
        }

        private bool CanRegister(object o)
        {
            return !string.IsNullOrWhiteSpace(Name) && 
                !string.IsNullOrWhiteSpace(Surname) &&
                !string.IsNullOrWhiteSpace(Username) &&
                !string.IsNullOrWhiteSpace(Password) &&
                !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private async void OnRegister(object o)
        {
            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Passwords not matching!");
                return;
            }

            UserModel newUser = new UserModel()
            {
                Name = Name,
                Surname = Surname,
                Username = Username,
            };
            try
            {
                await _userService.Create(newUser, Password);
                MessageBox.Show("Registration successful!");
                _navigateService.NavigateTo<LoginViewModel>();
            }
            catch (DatabaseResponseException e)
            {
                MessageBox.Show(e.Message);
            }
        }

    }
}
