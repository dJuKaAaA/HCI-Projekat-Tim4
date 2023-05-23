using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;

namespace TravelAgent.Service
{
    public class TouristAttractionService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public TouristAttractionService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<IEnumerable<TouristAttractionModel>> GetAll()
        {
            string touristAttractionTableAlias = "touristAttractionsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {touristAttractionTableAlias}.id, {touristAttractionTableAlias}.name, {touristAttractionTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.TouristAttractionsTableName} {touristAttractionTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {locationsTableAlias}.id = {touristAttractionTableAlias}.location_id";

            List<TouristAttractionModel> result = new List<TouristAttractionModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, reader =>
            {
                while (reader.Read())
                {
                    LocationModel location = new LocationModel()
                    {
                        Id = reader.GetInt32(3),
                        Address = reader.GetString(4),
                        Latitude = reader.GetDouble(5),
                        Longitude = reader.GetDouble(6),
                    };
                    TouristAttractionModel touristAttraction = new TouristAttractionModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Image = $"{_consts.PathToTouristAttractionsImages}/{reader.GetString(2)}",
                        Location = location
                    };
                    result.Add(touristAttraction);
                }
            });

            return result;
        }

        public async Task<IEnumerable<TouristAttractionModel>> GetForTrip(int tripId)
        {
            string touristAttractionTableAlias = "touristAttractionsTable";
            string tripsTouristAttractionsTableAlias = "tripsTouristAttractionsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {touristAttractionTableAlias}.id, {touristAttractionTableAlias}.name, {touristAttractionTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.TripsTouristAttractionsTableName} {tripsTouristAttractionsTableAlias}, " +
                $"{_consts.TouristAttractionsTableName} {touristAttractionTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {tripId} = {tripsTouristAttractionsTableAlias}.trip_id AND " +
                $"{locationsTableAlias}.id = {touristAttractionTableAlias}.location_id AND " +
                $"{touristAttractionTableAlias}.id = {tripsTouristAttractionsTableAlias}.tourist_attraction_id";

            List<TouristAttractionModel> result = new List<TouristAttractionModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, reader =>
            {
                while (reader.Read())
                {
                    LocationModel location = new LocationModel()
                    {
                        Id = reader.GetInt32(3),
                        Address = reader.GetString(4),
                        Latitude = reader.GetDouble(5),
                        Longitude = reader.GetDouble(6),
                    };
                    TouristAttractionModel touristAttraction = new TouristAttractionModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Image = $"{_consts.PathToTouristAttractionsImages}/{reader.GetString(2)}",
                        Location = location
                    };
                    result.Add(touristAttraction);
                }
            });

            return result;
        }
    }
}
