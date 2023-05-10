using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelAgent.Core
{
    public class Consts
    {
        private static readonly string _relativePathToDatabase = "../../../Database/database.db";

        public string ConnectionString { get; } = $"Data Source={_relativePathToDatabase}";
        public string DateTimeFormatString { get; } = "d.M.yyyy. H:m:s";
    }
}
