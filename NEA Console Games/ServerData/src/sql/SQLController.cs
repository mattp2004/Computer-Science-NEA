using ServerData.src.api;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.sql
{
    class SQLController
    {
        public OdbcConnection connection;
        public SQLController()
        {
            try
            {
                OdbcConnection connection = new System.Data.Odbc.OdbcConnection($"SERVER={DbConfig.HOSTNAME};PORT=3306;DATABASE={DbConfig.DATABASE};USER={DbConfig.USERNAME};PASSWORD={DbConfig.PASSWORD};OPTION=3;");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static string QueryParams(string query, string replace)
        {
            return query.Replace("?", replace);
        }

    }
}
