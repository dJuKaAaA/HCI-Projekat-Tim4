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

        public async Task<IEnumerable<TouristAttractionModel>> Search(HashSet<TouristAttractionSearchType> searchTypes, TouristAttractionSearchModel touristAttractionSearchModel)
        {
            string touristAttractionTableAlias = "touristAttractionsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {touristAttractionTableAlias}.id, {touristAttractionTableAlias}.name, {touristAttractionTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.TouristAttractionsTableName} {touristAttractionTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {locationsTableAlias}.id = {touristAttractionTableAlias}.location_id ";

            if (searchTypes.Contains(TouristAttractionSearchType.Name))
            {
                command += $"AND {touristAttractionTableAlias}.name LIKE '%{touristAttractionSearchModel.NameKeyword}%' ";
            }
            if (searchTypes.Contains(TouristAttractionSearchType.Address))
            {
                command += $"AND {locationsTableAlias}.address LIKE '%{touristAttractionSearchModel.AddressKeyword}%' ";
            }

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
                        Image = reader.GetString(2),
                        Location = location
                    };
                    result.Add(touristAttraction);
                }
            });

            return result;
        }

        public async Task Delete(int id)
        {
            string command = $"DELETE FROM {_consts.TripsTouristAttractionsTableName} " +
                $"WHERE tourist_attraction_id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            command = $"DELETE FROM {_consts.TouristAttractionsTableName} " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Modify(int id, TouristAttractionModel touristAttraction)
        {
            string command = $"UPDATE {_consts.TouristAttractionsTableName} " +
                $"SET name = '{touristAttraction.Name}', location_id = {touristAttraction.Location.Id}, " +
                $"image = '{touristAttraction.Image}' " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Create(TouristAttractionModel touristAttraction)
        {
            string command = $"INSERT INTO {_consts.TouristAttractionsTableName} (name, location_id, image) " +
                $"VALUES ('{touristAttraction.Name}', {touristAttraction.Location.Id}, '{touristAttraction.Image}')";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
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
                        Image = reader.GetString(2),
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
                        Image = reader.GetString(2),
                        Location = location
                    };
                    result.Add(touristAttraction);
                }
            });

            return result;
        }
    }
}
