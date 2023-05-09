using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgent.Core;
using TravelAgent.Exception;
using TravelAgent.MVVM.Model;

namespace TravelAgent.Service
{
    public class UserService
    {
        private readonly Consts _consts;
        private readonly DatabaseExecutionService _databaseExcecutionService;
        private readonly string _tableName = "users";

        public UserService(
            Consts consts,
            DatabaseExecutionService databaseExcecutionService)
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
            string command = $"SELECT * FROM {_tableName}";
            List<UserModel> collection = new List<UserModel>();
            await _databaseExcecutionService.ExecuteQueryCommand(_consts.ConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    UserModel user = ConvertToUser(reader);
                    collection.Add(user);
                }
            });
            return collection;
        }

        public async Task Login(string username, string password)
        {
            string command = $"SELECT * FROM {_tableName} WHERE username = '{username}' AND password = '{password}'";
            bool exists = false;
            await _databaseExcecutionService.ExecuteQueryCommand(_consts.ConnectionString, command, (reader) =>
            {
                exists = reader.Read();
            });

            if (!exists)
            {
                throw new DatabaseResponseException("Invalid credentials!");
            }
        }

        public async Task Create(UserModel user, string password)
        {
            string validationQuery = $"SELECT * FROM {_tableName} WHERE username = '{user.Username}'";
            bool valid = false;
            await _databaseExcecutionService.ExecuteQueryCommand(_consts.ConnectionString, validationQuery, (reader) =>
            {
                valid = !reader.Read();
            });

            if (!valid)
            {
                throw new DatabaseResponseException("Username is taken!");
            }
            
            string insertCommand = $"INSERT INTO {_tableName} (name, surname, username, password) " +
                $"VALUES ('{user.Name}', '{user.Surname}', '{user.Username}', '{password}')";
            await _databaseExcecutionService.ExecuteNonQueryCommand(_consts.ConnectionString, insertCommand);

        }
    }
}
