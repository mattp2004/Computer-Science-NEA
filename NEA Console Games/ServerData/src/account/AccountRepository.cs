using ServerData.src.network;
using ServerData.src.sql;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.account
{
    class AccountRepository
    {
        private static string SELECT_ACCOUNT_BY_UUID = "SELECT * FROM accounts WHERE uuid=?";
        private static string SELECT_RANK_BY_UUID = "SELECT Rank FROM accounts WHERE uuid=?";
        public SqlRepository sqlRepository;

        public AccountRepository(SqlRepository sqlRepository)
        {
            this.sqlRepository = sqlRepository;
        }

        public void LoadAccount(Client _client)
        {
            AccountSet resultSet = new AccountSet();

            OdbcDataReader results = sqlRepository.Select(SQLController.QueryParams(SELECT_ACCOUNT_BY_UUID, _client.uuid));

            resultSet.id = results.GetInt32(0);
            resultSet.uuid = results.GetString(1);
            resultSet.username = results.GetString(2);
            resultSet.password = results.GetString(3);
            Rank tempRank;
            Enum.TryParse(results.GetString(4), out tempRank);
            resultSet.tokens = int.Parse(results.GetString(5));
            resultSet.created = DateTime.Now;
            resultSet.lastjoined = DateTime.Now;

            Account a = new Account(resultSet, _client);
        }

        public class AccountSet
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
