using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;

namespace TravelAgent.Service
{
    public class RestaurantService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public RestaurantService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<IEnumerable<RestaurantModel>> Search(IEnumerable<RestaurantSearchType> searchTypes, RestaurantSearchModel restaurantSearchModel)
        {
            string restourantsTableAlias = "restaurantsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {restourantsTableAlias}.id, {restourantsTableAlias}.name, {restourantsTableAlias}.stars, {restourantsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.RestaurantsTableName} {restourantsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {locationsTableAlias}.id = {restourantsTableAlias}.location_id ";
            
            if (searchTypes.Contains(RestaurantSearchType.Name))
            {
                command += $"AND {restourantsTableAlias}.name LIKE '%{restaurantSearchModel.NameKeyword}%' ";
            }
            if (searchTypes.Contains(RestaurantSearchType.Address))
            {
                command += $"AND {locationsTableAlias}.address LIKE '%{restaurantSearchModel.AddressKeyword}%' ";
            }
            if (searchTypes.Contains(RestaurantSearchType.Stars))
            {
                command += $"AND {restourantsTableAlias}.stars = {restaurantSearchModel.Stars} ";
            }

            List<RestaurantModel> result = new List<RestaurantModel>();
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
                    RestaurantModel restaurant = new RestaurantModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Stars = reader.GetInt32(2),
                        Image = reader.GetString(3),
                        Location = location
                    };
                    result.Add(restaurant);
                }
            });

            return result;

        }

        public async Task Delete(int id)
        {
            string command = $"DELETE FROM {_consts.TripsRestaurantsTableName} " +
                $"WHERE restaurant_id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            command = $"DELETE FROM {_consts.RestaurantsTableName} " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Modify(int id, RestaurantModel restaurant)
        {
            string command = $"UPDATE {_consts.RestaurantsTableName} " +
                $"SET name = '{restaurant.Name}', stars = {restaurant.Stars}, location_id = {restaurant.Location.Id}, " +
                $"image = '{restaurant.Image}' " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Create(RestaurantModel restaurant)
        {
            string command = $"INSERT INTO {_consts.RestaurantsTableName} (name, stars, location_id, image) " +
                $"VALUES ('{restaurant.Name}', {restaurant.Stars}, {restaurant.Location.Id}, '{restaurant.Image}')";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task<IEnumerable<RestaurantModel>> GetAll()
        {
            string restourantsTableAlias = "restaurantsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {restourantsTableAlias}.id, {restourantsTableAlias}.name, {restourantsTableAlias}.stars, {restourantsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.RestaurantsTableName} {restourantsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {locationsTableAlias}.id = {restourantsTableAlias}.location_id";
            List<RestaurantModel> result = new List<RestaurantModel>();
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
                    RestaurantModel restaurant = new RestaurantModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Stars = reader.GetInt32(2),
                        Image = reader.GetString(3),
                        Location = location
                    };
                    result.Add(restaurant);
                }
            });

            return result;

        }

        public async Task<IEnumerable<RestaurantModel>> GetForTrip(int tripId)
        {
            string restourantsTableAlias = "restaurantsTable";
            string tripsRestaurantsTableAlias = "tripsRestaurantsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {restourantsTableAlias}.id, {restourantsTableAlias}.name, {restourantsTableAlias}.stars, {restourantsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.TripsRestaurantsTableName} {tripsRestaurantsTableAlias}, " +
                $"{_consts.RestaurantsTableName} {restourantsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {tripId} = {tripsRestaurantsTableAlias}.trip_id AND " +
                $"{locationsTableAlias}.id = {restourantsTableAlias}.location_id AND " +
                $"{restourantsTableAlias}.id = {tripsRestaurantsTableAlias}.restaurant_id";
            List<RestaurantModel> result = new List<RestaurantModel>();
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
                    RestaurantModel restaurant = new RestaurantModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Stars = reader.GetInt32(2),
                        Image = reader.GetString(3),
                        Location = location
                    };
                    result.Add(restaurant);
                }
            });

            return result;
        }
    }
}
