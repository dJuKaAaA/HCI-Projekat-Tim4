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
    public class UserFlightService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public UserFlightService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<IEnumerable<UserFlightModel>> GetForUser(int userId)
        {
            string command = $"SELECT usersTable.id, usersTable.name, usersTable.surname, usersTable.username, " +
                $"flightsTable.id, flightsTable.takeoff_date_time, flightsTable.landing_date_time, flightsTable.price, " +
                $"departuresTable.id, departuresTable.name, departuresTable.longitude, departuresTable.latitude, departuresTable.image, " +
                $"destinationsTable.id, destinationsTable.name, destinationsTable.longitude, destinationsTable.latitude, destinationsTable.image, " +
                $"usersFlightsTable.type " +
                $"FROM {_consts.UsersTableName} usersTable, {_consts.FlightsTableName} flightsTable, " +
                $"{_consts.UsersFlightsTableName} usersFlightsTable, {_consts.LocationsTableName} departuresTable, " +
                $"{_consts.LocationsTableName} destinationsTable " +
                $"WHERE usersFlightsTable.user_id = {userId} AND usersFlightsTable.flight_id = flightsTable.id " +
                $"AND flightsTable.departure_id = departuresTable.id AND flightsTable.destination_id = destinationsTable.id " +
                $"AND usersTable.id = {userId}";
            List<UserFlightModel> results = new List<UserFlightModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.ConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    UserModel user = new UserModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                        Username = reader.GetString(3),
                    };
                    LocationModel departure = new LocationModel()
                    {
                        Id = reader.GetInt32(8),
                        Name = reader.GetString(9),
                        Longitude = reader.GetFloat(10),
                        Latitude = reader.GetFloat(11),
                        Image = $"{_consts.PathToLocationImages}/{reader.GetString(12)}",
                    };
                    LocationModel destination = new LocationModel()
                    {
                        Id = reader.GetInt32(13),
                        Name = reader.GetString(14),
                        Longitude = reader.GetFloat(15),
                        Latitude = reader.GetFloat(16),
                        Image = $"{_consts.PathToLocationImages}/{reader.GetString(17)}",
                    };
                    FlightModel flight = new FlightModel()
                    {
                        Id = reader.GetInt32(4),
                        Departure = departure,
                        Destination = destination,
                        TakeoffDateTime = DateTime.ParseExact(reader.GetString(5), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        LandingDateTime = DateTime.ParseExact(reader.GetString(6), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        Price = reader.GetFloat(7),
                    };
                    UserFlightModel userFlight = new UserFlightModel()
                    {
                        User = user,
                        Flight = flight,
                        Type = (FlightInvoiceType)Enum.Parse(typeof(FlightInvoiceType), reader.GetString(18))
                    };

                    results.Add(userFlight);
                }
            });

            return results;
        }
    }
}
