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
    public class AllTouristAttractionsViewModel : Core.ViewModel
    {
        public ObservableCollection<TouristAttractionModel> AllTouristAttractions { get; set; }

        private Visibility _toolbarVisibility;

        public Visibility ToolbarVisibility
        {
            get { return _toolbarVisibility; }
            set { _toolbarVisibility = value; OnPropertyChanged(); }
        }

        public AllTouristAttractionsViewModel()
        {
            ToolbarVisibility = MainViewModel.SignedUser.Type == Core.UserType.Traveler ? Visibility.Collapsed : Visibility.Visible;

            AllTouristAttractions = new ObservableCollection<TouristAttractionModel>()
            {
                new TouristAttractionModel{
                    Id = 1,
                    Name = "Tourist attraction 1",
                },
                new TouristAttractionModel()
                {
                    Id = 2,
                    Name = "Tourist attraction 2",
                },
                new TouristAttractionModel()
                {
                    Id = 3,
                    Name = "Tourist attraction 3",
                },
            };
        }

    }
}
