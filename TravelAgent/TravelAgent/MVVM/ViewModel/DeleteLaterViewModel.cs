using Microsoft.Data.Sqlite;
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
    public class DeleteLaterViewModel : Core.ViewModel
    {
        private readonly Service.NavigationService _navigationService;
        private readonly UserService _userService;

        public ICommand GoToHomeCommand { get; }

        public DeleteLaterViewModel(
            Service.NavigationService navigationService,
            UserService userService)
        {
            _navigationService = navigationService;
            _userService = userService;

            GoToHomeCommand = new Core.RelayCommand((object o) => _navigationService.NavigateTo<HomeViewModel>(), (object o) => true);


            LoadAll();

        }

        private async void LoadAll()
        {
            TestCollection = new ObservableCollection<UserModel>();
            IEnumerable<object> users = await _userService.GetAll();
            foreach(UserModel user in users)
            {
                TestCollection.Add(user);
            }

        }

        public ObservableCollection<UserModel> TestCollection { get; set; }
    }
}
