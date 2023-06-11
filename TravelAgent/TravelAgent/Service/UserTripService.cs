using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravelAgent.Core;
using TravelAgent.Exception;
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

        public async Task<IEnumerable<UserTripModel>> Search(IEnumerable<UserTripSearchType> searchTypes, UserTripSearchModel userTripSearchModel)
        {
            string command = $"SELECT usersTable.id, usersTable.name, usersTable.surname, usersTable.username, usersTable.type, " +
                $"tripsTable.id, tripsTable.departure_date_time, tripsTable.arrival_date_time, tripsTable.price, " +
                $"departuresTable.id, departuresTable.address, departuresTable.longitude, departuresTable.latitude, " +
                $"destinationsTable.id, destinationsTable.address, destinationsTable.longitude, destinationsTable.latitude, " +
                $"usersTripsTable.type, usersTripsTable.purchase_date " +
                $"FROM {_consts.UsersTableName} usersTable, {_consts.TripsTableName} tripsTable, " +
                $"{_consts.UsersTripsTableName} usersTripsTable, {_consts.LocationsTableName} departuresTable, " +
                $"{_consts.LocationsTableName} destinationsTable " +
                $"WHERE usersTripsTable.trip_id = tripsTable.id AND usersTripsTable.user_id = usersTable.id " +
                $"AND tripsTable.departure_id = departuresTable.id AND tripsTable.destination_id = destinationsTable.id ";

            // placing the search criteria based on the "searchType" set
            if (searchTypes.Contains(UserTripSearchType.Departure))
            {
                command += $"AND departuresTable.address LIKE '%{userTripSearchModel.DepartureSearchKeyword}%' ";
            }
            if (searchTypes.Contains(UserTripSearchType.Destination))
            {
                command += $"AND destinationsTable.address LIKE '%{userTripSearchModel.DestinationSearchKeyword}%' ";
            }
            if (searchTypes.Contains(UserTripSearchType.DepartureDateTime))
            {
                string departureDateFormatted = userTripSearchModel.SelectedDepartureDate?.ToString($"{_consts.DateFormatString}");
                command += $"AND tripsTable.departure_date_time LIKE '{departureDateFormatted}%' ";
            }
            if (searchTypes.Contains(UserTripSearchType.ArrivalDateTime))
            {
                string arrivalDateFormatted = userTripSearchModel.SelectedArrivalDate?.ToString($"{_consts.DateFormatString}");
                command += $"AND tripsTable.arrival_date_time LIKE '{arrivalDateFormatted}%' ";
            }
            if (searchTypes.Contains(UserTripSearchType.Price))
            {
                command += $"AND tripsTable.Price BETWEEN {userTripSearchModel.StartPriceRange} AND {userTripSearchModel.EndPriceRange} ";
            }
            if (searchTypes.Contains(UserTripSearchType.PurchaseMonth))
            {
                command += $"AND usersTripsTable.purchase_date LIKE '%.{userTripSearchModel.SelectedMonthIndex}.{userTripSearchModel.Year}.%' ";
            }
            if (searchTypes.Contains(UserTripSearchType.Trip))
            {
                command += $"AND usersTripsTable.trip_id = {userTripSearchModel.TripId} ";
            }

            List<UserTripModel> results = new List<UserTripModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    UserModel user = new UserModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                        Username = reader.GetString(3),
                        Type = (UserType)Enum.Parse(typeof(UserType), reader.GetString(4))
                    };
                    LocationModel departure = new LocationModel()
                    {
                        Id = reader.GetInt32(9),
                        Address = reader.GetString(10),
                        Longitude = reader.GetFloat(11),
                        Latitude = reader.GetFloat(12),
                    };
                    LocationModel destination = new LocationModel()
                    {
                        Id = reader.GetInt32(13),
                        Address = reader.GetString(14),
                        Longitude = reader.GetFloat(15),
                        Latitude = reader.GetFloat(16),
                    };
                    TripModel trip = new TripModel()
                    {
                        Id = reader.GetInt32(5),
                        Departure = departure,
                        Destination = destination,
                        DepartureDateTime = DateTime.ParseExact(reader.GetString(6), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        ArrivalDateTime = DateTime.ParseExact(reader.GetString(7), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        Price = reader.GetFloat(8),
                    };
                    UserTripModel userTrip = new UserTripModel()
                    {
                        User = user,
                        Trip = trip,
                        Type = (TripInvoiceType)Enum.Parse(typeof(TripInvoiceType), reader.GetString(17)),
                        PurchaseDate = DateTime.ParseExact(reader.GetString(18), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                    };

                    results.Add(userTrip);
                }
            });

            return results;

        }

        public async Task Cancel(int userId, int tripId)
        {
            string command = $"SELECT * FROM {_consts.UsersTripsTableName} " +
                $"WHERE trip_id = {tripId} AND user_id = {userId} AND type = '{TripInvoiceType.Reserved.ToString()}'";
            bool exists = true;
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, (reader) =>
            {
                exists = reader.Read();
            });
            if (!exists)
            {
                throw new DatabaseResponseException("Acquired trip not found or it is already purchased!");
            }

            command = $"DELETE FROM {_consts.UsersTripsTableName} " +
                $"WHERE trip_id = {tripId} AND user_id = {userId}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task CreateNew(UserTripModel userTrip)
        {
            string command = $"SELECT * FROM {_consts.UsersTripsTableName} " +
                $"WHERE trip_id = {userTrip.Trip.Id} AND user_id = {userTrip.User.Id}";
            bool exists = false;
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, (reader) =>
            {
                exists = reader.Read();
            });

            if (exists)
            {
                throw new DatabaseResponseException("You already have this trip in your 'Acquired trips'!");
            }

            string purchaseDateFormatted = userTrip.PurchaseDate.ToString($"{_consts.DateTimeFormatString}");
            command = $"INSERT INTO {_consts.UsersTripsTableName} (user_id, trip_id, type, purchase_date) " +
                $"VALUES ({userTrip.User.Id}, {userTrip.Trip.Id}, '{userTrip.Type}', '{purchaseDateFormatted}')";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task PurchaseReserved(int userId, int tripId)
        {
            string command = $"SELECT * FROM {_consts.UsersTripsTableName} " +
                $"WHERE trip_id = {tripId} AND user_id = {userId} AND type = '{TripInvoiceType.Reserved.ToString()}'";
            bool exists = true;
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, (reader) =>
            {
                exists = reader.Read();
            });
            if (!exists)
            {
                throw new DatabaseResponseException("Acquired trip not found or it is already purchased!");
            }

            string purchaseDateFormatted = DateTime.Now.ToString($"{_consts.DateTimeFormatString}");
            command = $"UPDATE {_consts.UsersTripsTableName} " +
                $"SET type = '{TripInvoiceType.Purchased.ToString()}', purchase_date = '{purchaseDateFormatted}' " +
                $"WHERE trip_id = {tripId} AND user_id = {userId}";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);
        }

        public async Task<IEnumerable<UserTripModel>> GetAll()
        {
            string command = $"SELECT usersTable.id, usersTable.name, usersTable.surname, usersTable.username, usersTable.type, " +
                $"tripsTable.id, tripsTable.departure_date_time, tripsTable.arrival_date_time, tripsTable.price, " +
                $"departuresTable.id, departuresTable.address, departuresTable.longitude, departuresTable.latitude, " +
                $"destinationsTable.id, destinationsTable.address, destinationsTable.longitude, destinationsTable.latitude, " +
                $"usersTripsTable.type, usersTripsTable.purchase_date " +
                $"FROM {_consts.UsersTableName} usersTable, {_consts.TripsTableName} tripsTable, " +
                $"{_consts.UsersTripsTableName} usersTripsTable, {_consts.LocationsTableName} departuresTable, " +
                $"{_consts.LocationsTableName} destinationsTable " +
                $"WHERE usersTripsTable.trip_id = tripsTable.id AND usersTripsTable.user_id = usersTable.id " +
                $"AND tripsTable.departure_id = departuresTable.id AND tripsTable.destination_id = destinationsTable.id";
            List<UserTripModel> results = new List<UserTripModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    UserModel user = new UserModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                        Username = reader.GetString(3),
                        Type = (UserType)Enum.Parse(typeof(UserType), reader.GetString(4))
                    };
                    LocationModel departure = new LocationModel()
                    {
                        Id = reader.GetInt32(9),
                        Address = reader.GetString(10),
                        Longitude = reader.GetFloat(11),
                        Latitude = reader.GetFloat(12),
                    };
                    LocationModel destination = new LocationModel()
                    {
                        Id = reader.GetInt32(13),
                        Address = reader.GetString(14),
                        Longitude = reader.GetFloat(15),
                        Latitude = reader.GetFloat(16),
                    };
                    TripModel trip = new TripModel()
                    {
                        Id = reader.GetInt32(5),
                        Departure = departure,
                        Destination = destination,
                        DepartureDateTime = DateTime.ParseExact(reader.GetString(6), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        ArrivalDateTime = DateTime.ParseExact(reader.GetString(7), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        Price = reader.GetFloat(8),
                    };
                    UserTripModel userTrip = new UserTripModel()
                    {
                        User = user,
                        Trip = trip,
                        Type = (TripInvoiceType)Enum.Parse(typeof(TripInvoiceType), reader.GetString(17)),
                        PurchaseDate = DateTime.ParseExact(reader.GetString(18), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                    };

                    results.Add(userTrip);
                }
            });

            return results;
        }

        public async Task<IEnumerable<UserTripModel>> GetForUser(int userId)
        {
            string command = $"SELECT usersTable.id, usersTable.name, usersTable.surname, usersTable.username, usersTable.type, " +
                $"tripsTable.id, tripsTable.departure_date_time, tripsTable.arrival_date_time, tripsTable.price, " +
                $"departuresTable.id, departuresTable.address, departuresTable.longitude, departuresTable.latitude, " +
                $"destinationsTable.id, destinationsTable.address, destinationsTable.longitude, destinationsTable.latitude, " +
                $"usersTripsTable.type, usersTripsTable.purchase_date " +
                $"FROM {_consts.UsersTableName} usersTable, {_consts.TripsTableName} tripsTable, " +
                $"{_consts.UsersTripsTableName} usersTripsTable, {_consts.LocationsTableName} departuresTable, " +
                $"{_consts.LocationsTableName} destinationsTable " +
                $"WHERE usersTripsTable.user_id = {userId} AND usersTripsTable.trip_id = tripsTable.id " +
                $"AND tripsTable.departure_id = departuresTable.id AND tripsTable.destination_id = destinationsTable.id " +
                $"AND usersTable.id = {userId}";
            List<UserTripModel> results = new List<UserTripModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    UserModel user = new UserModel()
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                        Username = reader.GetString(3),
                        Type = (UserType)Enum.Parse(typeof(UserType), reader.GetString(4))
                    };
                    LocationModel departure = new LocationModel()
                    {
                        Id = reader.GetInt32(9),
                        Address = reader.GetString(10),
                        Longitude = reader.GetFloat(11),
                        Latitude = reader.GetFloat(12),
                    };
                    LocationModel destination = new LocationModel()
                    {
                        Id = reader.GetInt32(13),
                        Address = reader.GetString(14),
                        Longitude = reader.GetFloat(15),
                        Latitude = reader.GetFloat(16),
                    };
                    TripModel trip = new TripModel()
                    {
                        Id = reader.GetInt32(5),
                        Departure = departure,
                        Destination = destination,
                        DepartureDateTime = DateTime.ParseExact(reader.GetString(6), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        ArrivalDateTime = DateTime.ParseExact(reader.GetString(7), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        Price = reader.GetFloat(8),
                    };
                    UserTripModel userTrip = new UserTripModel()
                    {
                        User = user,
                        Trip = trip,
                        Type = (TripInvoiceType)Enum.Parse(typeof(TripInvoiceType), reader.GetString(17)),
                        PurchaseDate = DateTime.ParseExact(reader.GetString(18), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                    };

                    results.Add(userTrip);
                }
            });

            return results;
        }
    }
}
