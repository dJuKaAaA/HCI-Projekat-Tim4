using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.MVVM.Model;

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

        private UserModel ConvertToUser(SqliteDataReader reader)
        {
            return new UserModel()
            {
                Id = int.Parse(reader.GetString(0)),
                Name = reader.GetString(1),
                Surname = reader.GetString(2),
                Username = reader.GetString(3),
            };
        }

        public async Task<IEnumerable<UserModel>> GetAll()
        {
            string sqlQuery = $"SELECT * FROM {_tableName}";
            List<UserModel> collection = new List<UserModel>();
            await _databaseExcecutionService.ExecuteDatabaseCommand(_consts.ConnectionString, sqlQuery, (reader) =>
            {
                while (reader.Read())
                {
                    UserModel user = ConvertToUser(reader);
                    collection.Add(user);
                }
            });
            return collection;
        }

        public async Task<bool> Login(string username, string password)
        {
            string sqlQuery = $"SELECT * FROM {_tableName} WHERE username = '{username}' AND password = '{password}'";
            bool response = false;
            await _databaseExcecutionService.ExecuteDatabaseCommand(_consts.ConnectionString, sqlQuery, (reader) =>
            {
                response = reader.Read();
            });
            return response;
        }
    }
}
