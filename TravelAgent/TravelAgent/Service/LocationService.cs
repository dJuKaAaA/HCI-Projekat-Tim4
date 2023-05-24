using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;

namespace TravelAgent.Service
{
    public class LocationService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExecutionService;

        public LocationService(
            Consts consts,
            DatabaseExecutionService databaseExecutionService)
        {
            _consts = consts;
            _databaseExecutionService = databaseExecutionService;
        }

        public async Task<LocationModel> Create(LocationModel location)
        {
            string command = $"INSERT INTO {_consts.LocationsTableName} (latitude, longitude, address)" +
                $"VALUES ({location.Latitude}, {location.Longitude}, '{location.Address}')";
            await _databaseExecutionService.ExecuteNonQueryCommand(_consts.SqliteConnectionString, command);

            command = $"SELECT id, latitude, longitude, address FROM {_consts.LocationsTableName} " +
                $"WHERE id = (SELECT MAX(id) FROM {_consts.LocationsTableName})";
            LocationModel newLocation = new LocationModel();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, reader =>
            {
                if (reader.Read())
                {
                    newLocation.Id = reader.GetInt32(0);
                    newLocation.Latitude = reader.GetDouble(1);
                    newLocation.Longitude = reader.GetDouble(2);
                    newLocation.Address = reader.GetString(3);
                }
            });

            return newLocation;
        }

        public async Task<IEnumerable<LocationModel>> GetAll()
        {
            string command = $"SELECT * FROM {_consts.LocationsTableName}";
            List<LocationModel> result = new List<LocationModel>();
            await _databaseExecutionService.ExecuteQueryCommand(_consts.SqliteConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    LocationModel location = new LocationModel()
                    {
                        Id = reader.GetInt32(0),
                        Address = reader.GetString(1),
                        Longitude = reader.GetFloat(2),
                        Latitude = reader.GetFloat(3),
                    };
                    result.Add(location);
                }
            });
            return result;
        }
    }
}
