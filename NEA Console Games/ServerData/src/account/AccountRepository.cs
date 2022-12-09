using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using ServerData.src.data;
using ServerData.src.misc;
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
    public class AccountRepository
    {
        private static string SELECT_ACCOUNT_BY_UUID = "SELECT * FROM accounts WHERE uuid='?'";
        private static string SELECT_ACCOUNT_BY_USER = "SELECT * FROM accounts WHERE username='?'";
        private static string SELECT_RANK_BY_UUID = "SELECT Rank FROM accounts WHERE uuid='?'";
        public SqlRepository sqlRepository;

        public AccountRepository(SqlRepository sqlRepository)
        {
            this.sqlRepository = sqlRepository;
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

        public Account LoadAccount(Client _client)
        {
            MySqlDataReader reader = new MySqlCommand(SQLController.QueryParams(SELECT_ACCOUNT_BY_UUID, _client.uuid), sqlRepository.sqlController.connection).ExecuteReader();
            reader.Read();
            Account a = null;
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
                a = new Account(resultSet, _client);
            }
            reader.Close();
            try
            {
                return a;
            } catch(Exception e)
            {
                Util.Error(e);
                return null;
            }
        }
        public void CreateAccount(Account account)
        {
            string query = $"INSERT INTO `accounts` (`id`, `uuid`, `username`, `password`, `rank`, `tokens`, `created`, `lastjoined`) VALUES (NULL, '{account.GetUuid()}', '{account.GetUsername()}', '{account.GetPassword()}', '{account.GetRank()}', '{account.GetTokens()}', '{Util.SQLDate(DateTime.Now)}', '{Util.SQLDate(DateTime.Now)}')";
            MySqlCommand a = new MySqlCommand(query,sqlRepository.sqlController.connection);
            a.ExecuteNonQuery();
        }

        public void CreateAccount(string username, string password)
        {
            string query = $"INSERT INTO `accounts` (`id`, `uuid`, `username`, `password`, `rank`, `tokens`, `created`, `lastjoined`) VALUES (NULL, '{DataUtil.GenerateUUID()}', '{username}', '{password}', 'USER', '0', '{Util.SQLDate(DateTime.Now)}', '{Util.SQLDate(DateTime.Now)}')";
            MySqlCommand a = new MySqlCommand(query, sqlRepository.sqlController.connection);
            a.ExecuteNonQuery();
        }
    }
}
