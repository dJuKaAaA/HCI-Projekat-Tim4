using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravelAgent.MVVM.Model;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllRestorauntsViewModel : Core.ViewModel
    {
        public ObservableCollection<RestorauntModel> AllRestoraunts { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        public AllRestorauntsViewModel()
        {
            ToolbarVisibility = MainViewModel.SignedUser.Type == Core.UserType.Traveler ? Visibility.Collapsed : Visibility.Visible;

            AllRestoraunts = new ObservableCollection<RestorauntModel>()
            {
                new RestorauntModel()
                {
                    Id = 1,
                    Name = "Restoraunt1",
                    Stars = 3,
                    LocationId = 1
                },
                new RestorauntModel()
                {
                    Id = 2,
                    Name = "Restoraunt2",
                    Stars = 4,
                    LocationId = 1
                },
                new RestorauntModel()
                {
                    Id = 3,
                    Name = "Restoraunt3",
                    Stars = 5,
                    LocationId = 1
                },
                new RestorauntModel()
                {
                    Id = 4,
                    Name = "Restoraunt4",
                    Stars = 2,
                    LocationId = 1
                },
                new RestorauntModel()
                {
                    Id = 5,
                    Name = "Restoraunt5",
                    Stars = 1,
                    LocationId = 1
                },
            };
        }
    }
}
