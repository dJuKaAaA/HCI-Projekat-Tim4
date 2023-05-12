using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.MVVM.Model;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllRestorauntsViewModel : Core.ViewModel
    {
        public ObservableCollection<RestorauntModel> AllRestoraunts { get; set; }

        public AllRestorauntsViewModel()
        {
            AllRestoraunts = new ObservableCollection<RestorauntModel>()
            {
                new RestorauntModel()
                {
                    Id = 1,
                    Name = "Restoraunt1",
                    Stars = 5,
                    LocationId = 1
                },
                new RestorauntModel()
                {
                    Id = 2,
                    Name = "Restoraunt2",
                    Stars = 5,
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
                    Stars = 5,
                    LocationId = 1
                },
                new RestorauntModel()
                {
                    Id = 5,
                    Name = "Restoraunt5",
                    Stars = 5,
                    LocationId = 1
                },
            };
        }
    }
}
