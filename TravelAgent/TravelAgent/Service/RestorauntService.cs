using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;

namespace TravelAgent.Service
{
    public class RestorauntService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public RestorauntService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<IEnumerable<RestorauntModel>> GetAll()
        {
            string restourantsTableAlias = "restorauntsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {restourantsTableAlias}.id, {restourantsTableAlias}.name, {restourantsTableAlias}.stars, {restourantsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.RestorauntsTableName} {restourantsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {locationsTableAlias}.id = {restourantsTableAlias}.location_id";
            List<RestorauntModel> result = new List<RestorauntModel>();
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
                    RestorauntModel restoraunt = new RestorauntModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Stars = reader.GetInt32(2),
                        Image = $"{_consts.PathToRestorauntImages}/{reader.GetString(3)}",
                        Location = location
                    };
                    result.Add(restoraunt);
                }
            });

            return result;

        }

        public async Task<IEnumerable<RestorauntModel>> GetForTrip(int tripId)
        {
            string restourantsTableAlias = "restorauntsTable";
            string tripsRestorauntsTableAlias = "tripsRestorauntsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {restourantsTableAlias}.id, {restourantsTableAlias}.name, {restourantsTableAlias}.stars, {restourantsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.TripsRestorauntsTableName} {tripsRestorauntsTableAlias}, " +
                $"{_consts.RestorauntsTableName} {restourantsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {tripId} = {tripsRestorauntsTableAlias}.trip_id AND " +
                $"{locationsTableAlias}.id = {restourantsTableAlias}.location_id AND " +
                $"{restourantsTableAlias}.id = {tripsRestorauntsTableAlias}.restoraunt_id";
            List<RestorauntModel> result = new List<RestorauntModel>();
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
                    RestorauntModel restoraunt = new RestorauntModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Stars = reader.GetInt32(2),
                        Image = $"{_consts.PathToRestorauntImages}/{reader.GetString(3)}",
                        Location = location
                    };
                    result.Add(restoraunt);
                }
            });

            return result;
        }
    }
}
