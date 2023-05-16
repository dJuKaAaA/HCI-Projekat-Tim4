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
    public class FlightService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public FlightService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<IEnumerable<FlightModel>> GetAll()
        {
            string command = $"SELECT flightsTable.id, flightsTable.takeoff_date_time, flightsTable.landing_date_time, flightsTable.price, " +
                $"departuresTable.id, departuresTable.name, departuresTable.longitude, departuresTable.latitude, departuresTable.image, " +
                $"destinationsTable.id, destinationsTable.name, destinationsTable.longitude, destinationsTable.latitude, destinationsTable.image " +
                $" FROM {_consts.FlightsTableName} flightsTable, {_consts.LocationsTableName} departuresTable, " +
                $"{_consts.LocationsTableName} destinationsTable WHERE flightsTable.departure_id = departuresTable.id " +
                $"AND flightsTable.destination_id = destinationsTable.id";
            List<FlightModel> results = new List<FlightModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.ConnectionString, command, (reader) =>
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
                    FlightModel flight = new FlightModel()
                    {
                        Id = reader.GetInt32(0),
                        Departure = departure,
                        Destination = destination,
                        TakeoffDateTime = DateTime.ParseExact(reader.GetString(1), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        LandingDateTime = DateTime.ParseExact(reader.GetString(2), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        Price = reader.GetFloat(3),
                    };
                    results.Add(flight);
                }
            });

            return results;
        }
    }
}
