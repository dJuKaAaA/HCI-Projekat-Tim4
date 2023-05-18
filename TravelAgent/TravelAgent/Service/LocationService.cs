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
                        Name = reader.GetString(1),
                        Longitude = reader.GetFloat(2),
                        Latitude = reader.GetFloat(3),
                        Image = $"{_consts.PathToLocationImages}/{reader.GetString(4)}"
                    };
                    result.Add(location);
                }
            });
            return result;
        }
    }
}
