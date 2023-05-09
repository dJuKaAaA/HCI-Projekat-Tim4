using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Service
{
    public class DatabaseExecutionService
    {
        public async Task ExecuteQueryCommand(string connectionString, string queryCommand, Action<SqliteDataReader> action)
        {
            await Task.Run(() =>
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(queryCommand, connection);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        action(reader);
                    }
                }
            });
        }

        public async Task ExecuteNonQueryCommand(string connectionString, string nonQueryCommand)
        {
            await Task.Run(() =>
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(nonQueryCommand, connection);

                    command.ExecuteNonQuery();
                }
            });
        }
    }
}
