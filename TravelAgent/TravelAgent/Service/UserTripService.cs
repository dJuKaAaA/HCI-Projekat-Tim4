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
    public class UserTripService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public UserTripService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<IEnumerable<UserTripModel>> GetForUser(int userId)
        {
            string command = $"SELECT usersTable.id, usersTable.name, usersTable.surname, usersTable.username, " +
                $"tripsTable.id, tripsTable.departure_date_time, tripsTable.arrival_date_time, tripsTable.price, " +
                $"departuresTable.id, departuresTable.name, departuresTable.longitude, departuresTable.latitude, departuresTable.image, " +
                $"destinationsTable.id, destinationsTable.name, destinationsTable.longitude, destinationsTable.latitude, destinationsTable.image, " +
                $"usersTripsTable.type " +
                $"FROM {_consts.UsersTableName} usersTable, {_consts.TripsTableName} tripsTable, " +
                $"{_consts.UsersTripsTableName} usersTripsTable, {_consts.LocationsTableName} departuresTable, " +
                $"{_consts.LocationsTableName} destinationsTable " +
                $"WHERE usersTripsTable.user_id = {userId} AND usersTripsTable.trip_id = tripsTable.id " +
                $"AND tripsTable.departure_id = departuresTable.id AND tripsTable.destination_id = destinationsTable.id " +
                $"AND usersTable.id = {userId}";
            List<UserTripModel> results = new List<UserTripModel>();
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
                    TripModel trip = new TripModel()
                    {
                        Id = reader.GetInt32(4),
                        Departure = departure,
                        Destination = destination,
                        DepartureDateTime = DateTime.ParseExact(reader.GetString(5), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        ArrivalDateTime = DateTime.ParseExact(reader.GetString(6), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        Price = reader.GetFloat(7),
                    };
                    UserTripModel userTrip = new UserTripModel()
                    {
                        User = user,
                        Trip = trip,
                        Type = (TripInvoiceType)Enum.Parse(typeof(TripInvoiceType), reader.GetString(18))
                    };

                    results.Add(userTrip);
                }
            });

            return results;
        }
    }
}
