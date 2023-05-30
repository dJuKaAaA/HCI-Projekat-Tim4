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
        public ObservableCollection<RestaurantModel> AllRestaurants { get; set; }
        public ObservableCollection<AccommodationModel> AllAccommodations { get; set; }

        private readonly Service.AccommodationService _accommodationService;
        private readonly Service.TouristAttractionService _touristAttractionService;
        private readonly Service.RestaurantService _restaurantService;
        public MapService MapService { get; }
        public Consts Consts { get; }

        public MapViewModel(
            Service.AccommodationService accommodationService,
            Service.TouristAttractionService touristAttractionService,
            Service.RestaurantService restaurantService,
            MapService mapService,
            Core.Consts consts)
        {
            AllTouristAttractions = new ObservableCollection<TouristAttractionModel>();
            AllRestaurants = new ObservableCollection<RestaurantModel>();
            AllAccommodations = new ObservableCollection<AccommodationModel>();

            _accommodationService = accommodationService;
            _touristAttractionService = touristAttractionService;
            _restaurantService = restaurantService;
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

        public async Task LoadRestaurants()
        {
            AllRestaurants.Clear();
            IEnumerable<RestaurantModel> restaurants = await _restaurantService.GetAll();
            foreach (RestaurantModel restaurant in restaurants)
            {
                AllRestaurants.Add(restaurant);
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
