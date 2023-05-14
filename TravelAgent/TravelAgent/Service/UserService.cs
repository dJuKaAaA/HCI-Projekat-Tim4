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

        public async Task<IEnumerable<UserModel>> GetAll()
        {
            string command = $"SELECT * FROM {_tableName}";
            List<UserModel> collection = new List<UserModel>();
            await _databaseExcecutionService.ExecuteQueryCommand(_consts.ConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    UserModel user = new UserModel() 
                    { 
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1), 
                        Surname = reader.GetString(2), 
                        Username = reader.GetString(3)
                    };
                    collection.Add(user);
                }
            });
            return collection;
        }

        public async Task<UserModel> Login(string username, string password)
        {
            string command = $"SELECT * FROM {_tableName} WHERE username = '{username}' AND password = '{password}'";
            UserModel? user = null;
            await _databaseExcecutionService.ExecuteQueryCommand(_consts.ConnectionString, command, (reader) =>
            {
                while (reader.Read())
                {
                    user = new UserModel() 
                    { 
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1), 
                        Surname = reader.GetString(2), 
                        Username = reader.GetString(3)
                    };
                }
            });

            if (user == null)
            {
                throw new DatabaseResponseException("Invalid credentials!");
            }

            return user;
        }

        public async Task Create(UserModel user, string password)
        {
            string validationQuery = $"SELECT * FROM {_tableName} WHERE username = '{user.Username}'";
            bool taken = false;
            await _databaseExcecutionService.ExecuteQueryCommand(_consts.ConnectionString, validationQuery, (reader) =>
            {
                taken = reader.Read();
            });

            if (taken)
            {
                throw new DatabaseResponseException("Username is taken!");
            }
            
            string command = $"INSERT INTO {_tableName} (name, surname, username, password) " +
                $"VALUES ('{user.Name}', '{user.Surname}', '{user.Username}', '{password}')";
            await _databaseExcecutionService.ExecuteNonQueryCommand(_consts.ConnectionString, command);

        }
    }
}
