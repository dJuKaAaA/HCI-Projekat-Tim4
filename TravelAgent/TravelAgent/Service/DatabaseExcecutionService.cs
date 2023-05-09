using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Service
{
    public class DatabaseExcecutionService
    {
        public async Task ExecuteDatabaseCommand(string connectionString, string sqlQuery, Action<SqliteDataReader> action)
        {
            await Task.Run(() =>
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(sqlQuery, connection);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        action(reader);
                    }
                }
            });
        }
    }
}
