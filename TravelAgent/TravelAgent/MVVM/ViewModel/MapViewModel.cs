using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;
using TravelAgent.Service;

namespace TravelAgent.MVVM.ViewModel
{
    public class MapViewModel : Core.ViewModel
    {
        public ObservableCollection<TouristAttractionModel> AllTouristAttractions { get; set; }
        public ObservableCollection<RestorauntModel> AllRestoraunts { get; set; }
        public ObservableCollection<AccommodationModel> AllAccommodations { get; set; }

        private readonly Service.AccommodationService _accommodationService;
        private readonly Service.TouristAttractionService _touristAttractionService;
        private readonly Service.RestorauntService _restorauntService;
        public MapService MapService { get; }
        public Consts Consts { get; }

        public MapViewModel(
            Service.AccommodationService accommodationService,
            Service.TouristAttractionService touristAttractionService,
            Service.RestorauntService restorauntService,
            MapService mapService,
            Core.Consts consts)
        {
            AllTouristAttractions = new ObservableCollection<TouristAttractionModel>();
            AllRestoraunts = new ObservableCollection<RestorauntModel>();
            AllAccommodations = new ObservableCollection<AccommodationModel>();

            _accommodationService = accommodationService;
            _touristAttractionService = touristAttractionService;
            _restorauntService = restorauntService;
            MapService = mapService;
            Consts = consts;

        }

        public async Task LoadTouristAttractions()
        {
            AllTouristAttractions.Clear();
            IEnumerable<TouristAttractionModel> touristAttractions = await _touristAttractionService.GetAll();
            foreach (TouristAttractionModel touristAttraction in touristAttractions)
            {
                AllTouristAttractions.Add(touristAttraction);
            }
        }

        public async Task LoadRestoraunts()
        {
            AllRestoraunts.Clear();
            IEnumerable<RestorauntModel> restoraunts = await _restorauntService.GetAll();
            foreach (RestorauntModel restoraunt in restoraunts)
            {
                AllRestoraunts.Add(restoraunt);
            }
        }

        public async Task LoadAccommodations()
        {
            AllAccommodations.Clear();
            IEnumerable<AccommodationModel> accommodations = await _accommodationService.GetAll();
            foreach (AccommodationModel accommodation in accommodations)
            {
                AllAccommodations.Add(accommodation);
            }
        }


    }
}
