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
        public string BingMapsApiKey { get; } = "AlMtjm3qYzhPYIYWWq76wu7Be68h6ebShf43PYwn1RH8a05_Ksk_mz9_M5m71Rmr";
    }
}
