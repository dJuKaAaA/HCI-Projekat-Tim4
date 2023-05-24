using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;

namespace TravelAgent.Service
{
    public class AccommodationService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public AccommodationService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<IEnumerable<AccommodationModel>> GetAll()
        {
            string accommodationsTableAlias = "accommodationsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {accommodationsTableAlias}.id, {accommodationsTableAlias}.name, {accommodationsTableAlias}.rating, {accommodationsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.AccommodationsTableName} {accommodationsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {locationsTableAlias}.id = {accommodationsTableAlias}.location_id";
            List<AccommodationModel> result = new List<AccommodationModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, reader =>
            {
                while (reader.Read())
                {
                    LocationModel location = new LocationModel()
                    {
                        Id = reader.GetInt32(4),
                        Address = reader.GetString(5),
                        Latitude = reader.GetDouble(6),
                        Longitude = reader.GetDouble(7)
                    };
                    AccommodationModel accommodation = new AccommodationModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Rating = reader.GetDouble(2),
                        Image = $"{_consts.PathToAccommodationImages}/{reader.GetString(3)}",
                        Location = location
                    };
                    result.Add(accommodation);
                }
            });

            return result;

        }

        public async Task<IEnumerable<AccommodationModel>> GetForTrip(int tripId)
        {
            string accommodationsTableAlias = "accommodationsTable";
            string tripsAccommodationsTableAlias = "tripsAccomodationsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {accommodationsTableAlias}.id, {accommodationsTableAlias}.name, {accommodationsTableAlias}.rating, {accommodationsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.TripsAccommodationsTableName} {tripsAccommodationsTableAlias}, " +
                $"{_consts.AccommodationsTableName} {accommodationsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {tripId} = {tripsAccommodationsTableAlias}.trip_id AND " +
                $"{locationsTableAlias}.id = {accommodationsTableAlias}.location_id AND " +
                $"{accommodationsTableAlias}.id = {tripsAccommodationsTableAlias}.accommodation_id";
            List<AccommodationModel> result = new List<AccommodationModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, reader =>
            {
                while (reader.Read())
                {
                    LocationModel location = new LocationModel()
                    {
                        Id = reader.GetInt32(4),
                        Address = reader.GetString(5),
                        Latitude = reader.GetDouble(6),
                        Longitude = reader.GetDouble(7)
                    };
                    AccommodationModel accommodation = new AccommodationModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Rating = reader.GetDouble(2),
                        Image = $"{_consts.PathToAccommodationImages}/{reader.GetString(3)}",
                        Location = location
                    };
                    result.Add(accommodation);
                }
            });

            return result;
        }
    }
}
