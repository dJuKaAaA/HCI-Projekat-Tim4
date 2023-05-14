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
        private readonly string _tableName = "flights";

        public FlightService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<IEnumerable<FlightModel>> GetAll()
        {
            string joinedTableName = "locations";
            string command = $"SELECT * FROM {_tableName} t1, {joinedTableName} t2, {joinedTableName} t3 WHERE t1.departure_id = t2.id " +
                $"AND t1.destination_id = t3.id";
            List<FlightModel> results = new List<FlightModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.ConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    LocationModel departure = new LocationModel()
                    {
                        Id = reader.GetInt32(6),
                        Name = reader.GetString(7),
                        Longitude = reader.GetFloat(8),
                        Latitude = reader.GetFloat(9),
                        Image = $"{_consts.PathToLocationImages}/{reader.GetString(10)}",
                    };
                    LocationModel destination = new LocationModel()
                    {
                        Id = reader.GetInt32(11),
                        Name = reader.GetString(12),
                        Longitude = reader.GetFloat(13),
                        Latitude = reader.GetFloat(14),
                        Image = $"{_consts.PathToLocationImages}/{reader.GetString(15)}",
                    };
                    FlightModel flight = new FlightModel()
                    {
                        Id = reader.GetInt32(0),
                        Departure = departure,
                        Destination = destination,
                        TakeoffDateTime = DateTime.ParseExact(reader.GetString(3), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        LandingDateTime = DateTime.ParseExact(reader.GetString(4), _consts.DateTimeFormatString, CultureInfo.InvariantCulture),
                        Price = reader.GetFloat(5),
                    };
                    results.Add(flight);
                }
            });

            return results;
        }
    }
}
