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

        public async Task<IEnumerable<TripModel>> GetAll()
        {
            string command = $"SELECT tripsTable.id, tripsTable.departure_date_time, tripsTable.arrival_date_time, tripsTable.price, " +
                $"departuresTable.id, departuresTable.name, departuresTable.longitude, departuresTable.latitude, departuresTable.image, " +
                $"destinationsTable.id, destinationsTable.name, destinationsTable.longitude, destinationsTable.latitude, destinationsTable.image " +
                $" FROM {_consts.TripsTableName} tripsTable, {_consts.LocationsTableName} departuresTable, " +
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
                        Name = reader.GetString(5),
                        Longitude = reader.GetFloat(6),
                        Latitude = reader.GetFloat(7),
                        Image = $"{_consts.PathToLocationImages}/{reader.GetString(8)}",
                    };
                    LocationModel destination = new LocationModel()
                    {
                        Id = reader.GetInt32(9),
                        Name = reader.GetString(10),
                        Longitude = reader.GetFloat(11),
                        Latitude = reader.GetFloat(12),
                        Image = $"{_consts.PathToLocationImages}/{reader.GetString(13)}",
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
