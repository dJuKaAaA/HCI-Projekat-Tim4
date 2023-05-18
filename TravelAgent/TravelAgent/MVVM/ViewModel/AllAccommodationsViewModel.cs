﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.MVVM.Model;

namespace TravelAgent.MVVM.ViewModel
{
    public class AllAccommodationsViewModel : Core.ViewModel
    {
        public ObservableCollection<AccommodationModel> AllAccommodations { get; set; }

        public AllAccommodationsViewModel()
        {
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
