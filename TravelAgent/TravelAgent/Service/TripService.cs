using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;

namespace TravelAgent.Service
{
    public class TripService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public TripService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task Delete(int id)
        {
            string command = $"DELETE FROM {_consts.UsersTripsTableName} " +
                $"WHERE trip_id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            command = $"DELETE FROM {_consts.TripsTableName} " +
                $"WHERE id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task Modify(
            int id, 
            TripModel trip, 
            IEnumerable<RestorauntModel> restoraunts, 
            IEnumerable<AccommodationModel> accommodations,
            IEnumerable<TouristAttractionModel> touristAttractions)
        {
            string departureDateFormat = trip.DepartureDateTime.ToString($"{_consts.DateTimeFormatString}");
            string arrivalDateFormat = trip.ArrivalDateTime.ToString($"{_consts.DateTimeFormatString}");
            string command = $"UPDATE {_consts.TripsTableName} " +
                $"SET departure_id = {trip.Departure.Id}, destination_id = {trip.Destination.Id}, departure_date_time = '{departureDateFormat}', " +
                $"arrival_date_time = '{arrivalDateFormat}', price = {trip.Price} " +
                $"WHERE id = {id}";

            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);


            command = $"DELETE FROM {_consts.TripsRestorauntsTableName} " +
                $"WHERE trip_id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            foreach (RestorauntModel restoraunt in restoraunts)
            {
                command = $"INSERT INTO {_consts.TripsRestorauntsTableName} (trip_id, restoraunt_id) " +
                    $"VALUES ({id}, {restoraunt.Id})";
                await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            }

            command = $"DELETE FROM {_consts.TripsAccommodationsTableName} " +
                $"WHERE trip_id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            foreach (AccommodationModel accommodation in accommodations)
            {
                command = $"INSERT INTO {_consts.TripsAccommodationsTableName} (trip_id, accommodation_id) " +
                    $"VALUES ({id}, {accommodation.Id})";
                await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            }

            command = $"DELETE FROM {_consts.TripsTouristAttractionsTableName} " +
                $"WHERE trip_id = {id}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            foreach (TouristAttractionModel touristAttraction in touristAttractions)
            {
                command = $"INSERT INTO {_consts.TripsTouristAttractionsTableName} (trip_id, tourist_attraction_id) " +
                    $"VALUES ({id}, {touristAttraction.Id})";
                await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            }
        }

        public async Task Create(
            TripModel trip, 
            IEnumerable<RestorauntModel> restoraunts, 
            IEnumerable<AccommodationModel> accommodations,
            IEnumerable<TouristAttractionModel> touristAttractions)
        {
            string departureDateFormat = trip.DepartureDateTime.ToString($"{_consts.DateTimeFormatString}");
            string arrivalDateFormat = trip.ArrivalDateTime.ToString($"{_consts.DateTimeFormatString}");
            string command = $"INSERT INTO {_consts.TripsTableName} (departure_id, destination_id, departure_date_time, arrival_date_time, price)" +
                $"VALUES ({trip.Departure.Id}, {trip.Destination.Id}, '{departureDateFormat}', '{arrivalDateFormat}', {trip.Price})";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);

            command = $"SELECT MAX(id) FROM {_consts.TripsTableName}";
            int tripId = 0;
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, reader =>
            {
                if (reader.Read())
                {
                    tripId = reader.GetInt32(0);
                }
            });

            foreach (RestorauntModel restoraunt in restoraunts)
            {
                command = $"INSERT INTO {_consts.TripsRestorauntsTableName} (trip_id, restoraunt_id) " +
                    $"VALUES ({tripId}, {restoraunt.Id})";
                await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            }

            foreach (AccommodationModel accommodation in accommodations)
            {
                command = $"INSERT INTO {_consts.TripsAccommodationsTableName} (trip_id, accommodation_id) " +
                    $"VALUES ({tripId}, {accommodation.Id})";
                await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            }

            foreach (TouristAttractionModel touristAttraction in touristAttractions)
            {
                command = $"INSERT INTO {_consts.TripsTouristAttractionsTableName} (trip_id, tourist_attraction_id) " +
                    $"VALUES ({tripId}, {touristAttraction.Id})";
                await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
            }
        }

        public async Task<IEnumerable<TripModel>> GetAll()
        {
            string command = $"SELECT tripsTable.id, tripsTable.departure_date_time, tripsTable.arrival_date_time, tripsTable.price, " +
                $"departuresTable.id, departuresTable.address, departuresTable.longitude, departuresTable.latitude, " +
                $"destinationsTable.id, destinationsTable.address, destinationsTable.longitude, destinationsTable.latitude " +
                $"FROM {_consts.TripsTableName} tripsTable, {_consts.LocationsTableName} departuresTable, " +
                $"{_consts.LocationsTableName} destinationsTable WHERE tripsTable.departure_id = departuresTable.id " +
                $"AND tripsTable.destination_id = destinationsTable.id";
            List<TripModel> results = new List<TripModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    LocationModel departure = new LocationModel()
                    {
                        Id = reader.GetInt32(4),
                        Address = reader.GetString(5),
                        Longitude = reader.GetFloat(6),
                        Latitude = reader.GetFloat(7),
                    };
                    LocationModel destination = new LocationModel()
                    {
                        Id = reader.GetInt32(8),
                        Address = reader.GetString(9),
                        Longitude = reader.GetFloat(10),
                        Latitude = reader.GetFloat(11),
                    };
                    TripModel trip = new TripModel()
                    {
                        Id = reader.GetInt32(0),
                        Departure = departure,
                        Destination = destination,
                        DepartureDateTime = DateTime.ParseExact(reader.GetString(1), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        ArrivalDateTime = DateTime.ParseExact(reader.GetString(2), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        Price = reader.GetFloat(3),
                    };
                    results.Add(trip);
                }
            });

            return results;
        }
    }
}
