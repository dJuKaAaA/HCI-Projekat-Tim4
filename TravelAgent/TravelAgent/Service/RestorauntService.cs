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

        public async Task<IEnumerable<RestorauntModel>> Search(HashSet<RestorauntSearchType> searchTypes, RestorauntSearchModel restorauntSearchModel)
        {
            string restourantsTableAlias = "restorauntsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {restourantsTableAlias}.id, {restourantsTableAlias}.name, {restourantsTableAlias}.stars, {restourantsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.RestorauntsTableName} {restourantsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {locationsTableAlias}.id = {restourantsTableAlias}.location_id ";
            
            if (searchTypes.Contains(RestorauntSearchType.Name))
            {
                command += $"AND {restourantsTableAlias}.name LIKE '%{restorauntSearchModel.NameKeyword}%' ";
            }
            if (searchTypes.Contains(RestorauntSearchType.Address))
            {
                command += $"AND {locationsTableAlias}.address LIKE '%{restorauntSearchModel.AddressKeyword}%' ";
            }
            if (searchTypes.Contains(RestorauntSearchType.Stars))
            {
                command += $"AND {restourantsTableAlias}.stars = {restorauntSearchModel.Stars} ";
            }

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
                        Image = reader.GetString(3),
                        Location = location
                    };
                    result.Add(restoraunt);
                }
            });

            return result;

        }

        public async Task Delete(int id)
        {
            string command = $"DELETE FROM {_consts.TripsRestorauntsTableName} " +
                $"WHERE restoraunt_id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            command = $"DELETE FROM {_consts.RestorauntsTableName} " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Modify(int id, RestorauntModel restoraunt)
        {
            string command = $"UPDATE {_consts.RestorauntsTableName} " +
                $"SET name = '{restoraunt.Name}', stars = {restoraunt.Stars}, location_id = {restoraunt.Location.Id}, " +
                $"image = '{restoraunt.Image}' " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Create(RestorauntModel restoraunt)
        {
            string command = $"INSERT INTO {_consts.RestorauntsTableName} (name, stars, location_id, image) " +
                $"VALUES ('{restoraunt.Name}', {restoraunt.Stars}, {restoraunt.Location.Id}, '{restoraunt.Image}')";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
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
                        Image = reader.GetString(3),
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
                        Image = reader.GetString(3),
                        Location = location
                    };
                    result.Add(restoraunt);
                }
            });

            return result;
        }
    }
}
