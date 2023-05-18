using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Core
{
    public class Consts
    {
        public string RelativePathRoot { get; }
        public string PathToDatabase { get; }

        // path to images
        public string PathToLocationImages { get; }
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

        public Consts()
        {
            RelativePathRoot = "../../..";
            PathToDatabase = $"{RelativePathRoot}/Database/database.db";
            PathToLocationImages = $"{RelativePathRoot}/Image/Location";
            PathToRestorauntImages = $"{RelativePathRoot}/Image/Restoraunt";
            PathToIcons = $"{RelativePathRoot}/Image/Icon";
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
        }
    }
}
