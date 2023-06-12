﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Core
{
    public class Consts
    {
        public string ProjectRootRelativePath { get; }
        public string PathToDatabase { get; }

        public string SqliteConnectionString { get; }
        public string DateFormatString { get; }
        public string TimeFormatString { get; }
        public string DateTimeFormatString { get; }
        public string BingMapsApiKey { get; }

        // path to images
        public string PathToTouristAttractionsImages { get; }
        public string PathToRestaurantImages { get; }
        public string PathToAccommodationImages { get; }
        public string PathToIcons { get; }

        // table names
        public string UsersTableName { get; }
        public string TripsTableName { get; }
        public string LocationsTableName { get; }
        public string RestaurantsTableName { get; }
        public string AccommodationsTableName { get; }
        public string TouristAttractionsTableName { get; }
        public string UsersTripsTableName { get; }
        public string TripsTouristAttractionsTableName { get; }
        public string TripsRestaurantsTableName { get; }
        public string TripsAccommodationsTableName { get; }

        // pushpins
        public string RestaurantPushpinIcon { get; }
        public string AccommodationPushpinIcon { get; }
        public string TouristAttractionPushpinIcon { get; }

        public Consts()
        {
            ProjectRootRelativePath = "../../..";
            PathToDatabase = $"{ProjectRootRelativePath}/Database/database.db";
            SqliteConnectionString = $"Data Source={PathToDatabase}";
            DateFormatString = "d.M.yyyy.";
            TimeFormatString = "H:m:s";
            DateTimeFormatString  = $"{DateFormatString} {TimeFormatString}";
            BingMapsApiKey  = "AlMtjm3qYzhPYIYWWq76wu7Be68h6ebShf43PYwn1RH8a05_Ksk_mz9_M5m71Rmr";

            PathToIcons = $"{ProjectRootRelativePath}/Image/Icon";
            PathToTouristAttractionsImages = $"{ProjectRootRelativePath}/Image/TouristAttraction";
            PathToRestaurantImages = $"{ProjectRootRelativePath}/Image/Restaurant";
            PathToAccommodationImages = $"{ProjectRootRelativePath}/Image/Accommodation";

            UsersTableName = "users";
            TripsTableName = "trips";
            LocationsTableName = "locations";
            RestaurantsTableName = "restaurants";
            AccommodationsTableName = "accommodations";
            TouristAttractionsTableName = "tourist_attractions";
            UsersTripsTableName = "users_trips";
            TripsTouristAttractionsTableName = "trips_tourist_attractions";
            TripsRestaurantsTableName = "trips_restaurants";
            TripsAccommodationsTableName = "trips_accommodations";

            RestaurantPushpinIcon = "restaurant-pushpin.png";
            AccommodationPushpinIcon = "accommodation-pushpin.png";
            TouristAttractionPushpinIcon = "tourist-attraction-pushpin.png";
        }
    }
}
