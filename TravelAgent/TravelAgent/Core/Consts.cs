using System;
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

        // path to images
        public string PathToTouristAttractionsImages { get; }
        public string PathToRestorauntImages { get; }
        public string PathToIcons { get; }
        
        public string SqliteConnectionString { get; }
        public string DateTimeFormatString { get; }
        public string BingMapsApiKey { get; }

        // table names
        public string UsersTableName { get; }
        public string TripsTableName { get; }
        public string LocationsTableName { get; }
        public string RestorauntsTableName { get; }
        public string AccomodationsTableName { get; }
        public string TouristAttractionsTableName { get; }
        public string UsersTripsTableName { get; }
        public string TripsTouristAttractionsTableName { get; }

        public Consts()
        {
            ProjectRootRelativePath = "../../..";
            PathToDatabase = $"{ProjectRootRelativePath}/Database/database.db";
            PathToTouristAttractionsImages = $"{ProjectRootRelativePath}/Image/TouristAttraction";
            PathToRestorauntImages = $"{ProjectRootRelativePath}/Image/Restoraunt";
            PathToIcons = $"{ProjectRootRelativePath}/Image/Icon";
            SqliteConnectionString = $"Data Source={PathToDatabase}";
            DateTimeFormatString  = "d.M.yyyy. H:m:s";
            BingMapsApiKey  = "AlMtjm3qYzhPYIYWWq76wu7Be68h6ebShf43PYwn1RH8a05_Ksk_mz9_M5m71Rmr";

            UsersTableName = "users";
            TripsTableName = "trips";
            LocationsTableName = "locations";
            RestorauntsTableName = "restoraunts";
            AccomodationsTableName = "accomodations";
            TouristAttractionsTableName = "tourist_attractions";
            UsersTripsTableName = "users_trips";
            TripsTouristAttractionsTableName = "trips_tourist_attractions";
        }
    }
}
