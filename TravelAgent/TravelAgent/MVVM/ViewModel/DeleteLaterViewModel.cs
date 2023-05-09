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

namespace TravelAgent.MVVM.ViewModel
{
    public class DeleteLaterViewModel : Core.ViewModel
    {
        private readonly Service.NavigationService _navigationService;
        private readonly Consts _consts;

        public ICommand GoToHomeCommand { get; }

        public DeleteLaterViewModel(
            Service.NavigationService navigationService,
            Consts consts)
        {
            _navigationService = navigationService;
            _consts = consts;

            GoToHomeCommand = new Core.RelayCommand((object o) => _navigationService.NavigateTo<HomeViewModel>(), (object o) => true);
            FillCollection();

        }

        private void FillCollection()
        {
            using (SqliteConnection connection = new SqliteConnection(_consts.ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM users";
                SqliteCommand command = new SqliteCommand(sql, connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TestCollection.Add(new
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2)
                    });
                }
                reader.Close();
            }
        }

        public ObservableCollection<object> TestCollection { get; set; } = new ObservableCollection<object>();
    }
}
