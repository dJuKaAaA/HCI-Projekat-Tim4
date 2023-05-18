using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.MVVM.Model;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllTouristAttractionsViewModel : Core.ViewModel
    {
        public ObservableCollection<TouristAttractionModel> AllTouristAttractions { get; set; }

        public AllTouristAttractionsViewModel()
        {
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
