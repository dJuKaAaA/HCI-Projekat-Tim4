using System;
using System.Collections.Generic;
using System.DirectoryServices;
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

        public async Task<IEnumerable<AccommodationModel>> Search(IEnumerable<AccommodationSearchType> searchTypes, AccommodationSearchModel accommodationSearchModel)
        {
            string accommodationsTableAlias = "accommodationsTable";
            string locationsTableAlias = "locationsTable";
            string command = $"SELECT {accommodationsTableAlias}.id, {accommodationsTableAlias}.name, {accommodationsTableAlias}.rating, {accommodationsTableAlias}.image, " +
                $"{locationsTableAlias}.id, {locationsTableAlias}.address, {locationsTableAlias}.latitude, {locationsTableAlias}.longitude " +
                $"FROM {_consts.AccommodationsTableName} {accommodationsTableAlias}, " +
                $"{_consts.LocationsTableName} {locationsTableAlias} " +
                $"WHERE {locationsTableAlias}.id = {accommodationsTableAlias}.location_id ";
            if (searchTypes.Contains(AccommodationSearchType.Name))
            {
                command += $"AND {accommodationsTableAlias}.name LIKE '%{accommodationSearchModel.NameKeyword}%' ";
            }
            if (searchTypes.Contains(AccommodationSearchType.Address))
            {
                command += $"AND {locationsTableAlias}.address LIKE '%{accommodationSearchModel.AddressKeyword}%' ";
            }
            if (searchTypes.Contains(AccommodationSearchType.Rating))
            {
                command += $"AND {accommodationsTableAlias}.rating BETWEEN {accommodationSearchModel.StartRatingRange} AND {accommodationSearchModel.EndRatingRange} ";
            }

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
                        Image = reader.GetString(3),
                        Location = location
                    };
                    result.Add(accommodation);
                }
            });

            return result;

        }

        public async Task Delete(int id)
        {
            string command = $"DELETE FROM {_consts.TripsAccommodationsTableName} " +
                $"WHERE accommodation_id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            command = $"DELETE FROM {_consts.AccommodationsTableName} " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Modify(int id, AccommodationModel accommodation)
        {
            string command = $"UPDATE {_consts.AccommodationsTableName} " +
                $"SET name = '{accommodation.Name}', rating = {accommodation.Rating}, location_id = {accommodation.Location.Id}, " +
                $"image = '{accommodation.Image}' " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Create(AccommodationModel accommodation)
        {
            string command = $"INSERT INTO {_consts.AccommodationsTableName} (name, rating, location_id, image) " +
                $"VALUES ('{accommodation.Name}', {accommodation.Rating}, {accommodation.Location.Id}, '{accommodation.Image}')";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
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
                        Image = reader.GetString(3),
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
                        Image = reader.GetString(3),
                        Location = location
                    };
                    result.Add(accommodation);
                }
            });

            return result;
        }
    }
}
