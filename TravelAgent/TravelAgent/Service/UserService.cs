using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;

namespace TravelAgent.Service
{
    public class UserService
    {
        private readonly Consts _consts;
        private readonly DatabaseExcecutionService _databaseExcecutionService;
        private readonly string _tableName = "users";

        public UserService(
            Consts consts,
            DatabaseExcecutionService databaseExcecutionService)
        {
            _consts = consts;
            _databaseExcecutionService = databaseExcecutionService;
        }

        public async Task<IEnumerable<object>> GetAll()
        {
            string sqlQuery = $"SELECT * FROM {_tableName}";
            List<object> collection = new List<object>();
            await _databaseExcecutionService.ExecuteDatabaseCommand(_consts.ConnectionString, sqlQuery, (reader) =>
            {
                while (reader.Read())
                {
                    collection.Add(new
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                    });
                }
            });
            return collection;
        }
    }
}
