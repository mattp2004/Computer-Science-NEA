using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using ServerData.src.data;
using ServerData.src.misc;
using ServerData.src.network;
using ServerData.src.sql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.account
{
    [Serializable]
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
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class AccountRepository
    {
        private static string SELECT_ACCOUNT_BY_UUID = "SELECT * FROM Accounts WHERE uuid='?'";
        private static string SELECT_ACCOUNT_BY_USER = "SELECT * FROM Accounts WHERE username='?'";
        private static string SELECT_RANK_BY_UUID = "SELECT Rank FROM Accounts WHERE uuid='?'";
        private static string SELECT_USERNAME_BY_USERNAME = "SELECT username FROM Accounts WHERE username='?'";
        public SqlRepository sqlRepository;

        public AccountRepository(SqlRepository sqlRepository)
        {
            this.sqlRepository = sqlRepository;
        }

        public bool AccountExists(string username)
        {
            MySqlDataReader reader = new MySqlCommand(SQLController.QueryParams(SELECT_USERNAME_BY_USERNAME, username), sqlRepository.sqlController.connection).ExecuteReader();
            reader.Read();
            return reader.HasRows;
        }

        public AccountSet GetAccountFromUsername(string username)
        {
            MySqlDataReader reader = new MySqlCommand(SQLController.QueryParams(SELECT_ACCOUNT_BY_USER, username), sqlRepository.sqlController.connection).ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                AccountSet resultSet = new AccountSet();
                resultSet.id = int.Parse(reader["id"].ToString());
                resultSet.uuid = reader["uuid"].ToString();
                resultSet.username = reader["username"].ToString();
                resultSet.password = reader["password"].ToString();
                Rank rank;
                Enum.TryParse(reader["rank"].ToString(), out rank);
                resultSet.tokens = int.Parse(reader["tokens"].ToString());
                resultSet.created = DateTime.Now;
                resultSet.lastjoined = DateTime.Now;
                reader.Close();
                return resultSet;
            }
            return null;
        }

        public void GiveTokens(Account acc, int num)
        {
            string query = $"UPDATE Accounts SET tokens = tokens + {num} WHERE uuid = '{acc.GetUuid()}'";
            MySqlCommand a = new MySqlCommand(query, sqlRepository.sqlController.connection);
            a.ExecuteNonQuery();
        }

        public Account GetAccountFromSet(AccountSet a)
        {
            return new Account(a, new Client(new System.Net.Sockets.TcpClient(), ""));
        }

        public Account LoadAccount(Client _client)
        {
            MySqlDataReader reader = new MySqlCommand(SQLController.QueryParams(SELECT_ACCOUNT_BY_UUID, _client.uuid), sqlRepository.sqlController.connection).ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                AccountSet resultSet = new AccountSet();
                resultSet.id = int.Parse(reader["id"].ToString());
                resultSet.uuid = reader["uuid"].ToString();
                resultSet.username = reader["username"].ToString();
                resultSet.password = reader["password"].ToString();
                Rank rank;
                try
                {
                    Enum.TryParse(reader["rank"].ToString(), out rank);
                    resultSet.rank = rank;
                }
                catch (Exception e) { resultSet.rank = Rank.USER; }
                resultSet.tokens = int.Parse(reader["tokens"].ToString());
                resultSet.created = DateTime.Now;
                resultSet.lastjoined = DateTime.Now;
                Account a = new Account(resultSet, _client);
                return a;
            }
            return null;
            reader.Close();
        }
        public void CreateAccount(Account account)
        {
            string query = $"INSERT INTO `Accounts` (`id`, `uuid`, `username`, `password`, `rank`, `tokens`, `created`, `lastjoined`) VALUES (NULL, '{account.GetUuid()}', '{account.GetUsername()}', '{account.GetPassword()}', '{account.GetRank()}', '{account.GetTokens()}', '{Util.SQLDate(DateTime.Now)}', '{Util.SQLDate(DateTime.Now)}')";
            MySqlCommand a = new MySqlCommand(query,sqlRepository.sqlController.connection);
            a.ExecuteNonQuery();
        }

        public void CreateAccount(string username, string password)
        {
            string query = $"INSERT INTO `Accounts` (`id`, `uuid`, `username`, `password`, `rank`, `tokens`, `created`, `lastjoined`) VALUES (NULL, '{DataUtil.GenerateUUID()}', '{username}', '{password}', 'USER', '0', '{Util.SQLDate(DateTime.Now)}', '{Util.SQLDate(DateTime.Now)}')";
            MySqlCommand a = new MySqlCommand(query, sqlRepository.sqlController.connection);
            a.ExecuteNonQuery();
        }
    }
}
