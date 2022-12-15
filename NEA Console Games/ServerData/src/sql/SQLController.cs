using MySql.Data.MySqlClient;
using ServerData.src.api;
using ServerData.src.api.Controllers.util;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.sql
{
    public class SQLController
    {
        public MySqlConnection connection;
        public SQLController()
        {
            try
            {
                connection = new MySqlConnection();
                connection.ConnectionString = $"SERVER={DbConfig.HOSTNAME};port=3306;Database={DbConfig.DATABASE};uid={DbConfig.USERNAME};pwd={DbConfig.PASSWORD};";
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    TextUtil.WriteLine($"Connected to MariaDB.", ConsoleColor.Green);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR# " + e.Message);
            }
        }
        public static string QueryParams(string query, string replace)
        {
            return query.Replace("?", replace);
        }

    }
}
