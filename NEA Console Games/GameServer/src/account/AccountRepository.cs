using GameServer.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using GameServer.src.data.MySQL;

namespace GameServer.src.account
{
    class AccountRepository
    {
        private static string SELECT_ACCOUNT_BY_UUID = "SELECT * FROM accounts WHERE uuid=?";
        private static string SELECT_RANK_BY_UUID = "SELECT Rank FROM accounts WHERE uuid=?";
        public static SQLController SqlC;

        public static void LoadAccount(Client _client)
        {
            //fetch data from db and store in result set
            ResultSet resultSet = new ResultSet();
            SqlC = new SQLController();
            OdbcConnection conn = new System.Data.Odbc.OdbcConnection("DRIVER={MySQL ODBC 5.3 ANSI Driver};SERVER=localhost;PORT=3306;DATABASE=data;USER=GameServer;PASSWORD=YZ3L0AYN5cMCSOQZ;OPTION=3;");
            conn.Open();
            OdbcCommand sqlToRun = new OdbcCommand(SqlC.QueryParams(SELECT_ACCOUNT_BY_UUID, _client.uuid), conn);
            OdbcDataReader results = sqlToRun.ExecuteReader();
            resultSet.id = results.GetInt32(0);
            resultSet.uuid = results.GetString(1);
            resultSet.username = results.GetString(2);
            resultSet.password = results.GetString(3);
            Rank tempRank;
            Enum.TryParse(results.GetString(4), out tempRank);
            resultSet.tokens = int.Parse(results.GetString(5));
            resultSet.created = DateTime.Now;
            resultSet.lastjoined = DateTime.Now;
            //resultSet.created = DateTime.Parse()
            Account a = new Account(resultSet, _client);
        }

        public Rank GetRank()
        {
            return Rank.OWNER;
        }

        public class ResultSet
        {
            public int id;
            public string uuid;
            public string username;
            public string password;
            public Rank rank;
            public int tokens;
            public DateTime created;
            public DateTime lastjoined;
        }
    }
}
