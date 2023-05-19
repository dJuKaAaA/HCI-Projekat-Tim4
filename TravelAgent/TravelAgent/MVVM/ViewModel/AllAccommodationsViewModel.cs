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
    public class AllAccommodationsViewModel : Core.ViewModel
    {
        public ObservableCollection<AccommodationModel> AllAccommodations { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        public AllAccommodationsViewModel()
        {
            ToolbarVisibility = MainViewModel.SignedUser?.Type == Core.UserType.Agent ? Visibility.Visible : Visibility.Collapsed;

            AllAccommodations = new ObservableCollection<AccommodationModel>()
            {
                new AccommodationModel()
                {
                    Id = 1,
                    Name = "Accommodation1",
                    Address = "Address1",
                    Rating = 5
                },
                new AccommodationModel()
                {
                    Id = 2,
                    Name = "Accommodation2",
                    Address = "Address2",
                    Rating = 5
                },
                new AccommodationModel()
                {
                    Id = 3,
                    Name = "Accommodation3",
                    Address = "Address3",
                    Rating = 5
                },
                new AccommodationModel()
                {
                    Id = 4,
                    Name = "Accommodation4",
                    Address = "Address4",
                    Rating = 5
                },
                new AccommodationModel()
                {
                    Id = 5,
                    Name = "Accommodation5",
                    Address = "Address5",
                    Rating = 5
                },
            };
        }

    }
}
