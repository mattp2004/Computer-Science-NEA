using MySql.Data.MySqlClient;
using ServerData.src.data;
using ServerData.src.sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerData.src.stats
{
    public class StatsRepository
    {
        public SQLController sqlController;

        public StatsRepository(SQLController sql)
        {
            sqlController = sql;
        }

        public Stats GetStatsFromUsername(string username)
        {
            Stats stats = new Stats();
            //Wins
            foreach (var name in Enum.GetNames(typeof(Games)))
            {
                string output = "";
                MySqlDataReader reader = new MySqlCommand($"SELECT Accounts.username, GameType.GameName, COUNT(*), Place FROM GamePlayers Join Accounts ON GamePlayers.Accounts_ID = Accounts.id Join GameInstance on GameInstance.id = GamePlayers.GameInstance_ID Join GameType ON GameType.id = GameInstance.GameType_ID where Accounts.username = '{username}' and GameType.GameName = '{name}' and GamePlayers.Place = 1 GROUP BY Accounts.username, GameType.GameName, Place;", sqlController.connection).ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    output = reader["COUNT(*)"].ToString();
                }
                int num;
                int.TryParse(output, out num);
                Games a;
                Enum.TryParse(name, out a);
                stats.GameWins.Add(a, num);
                reader.Close();
                reader.Dispose();
                Thread.Sleep(10);
            }
            Thread.Sleep(10);

            //Loses
            foreach (var name in Enum.GetNames(typeof(Games)))
            {
                string output = "";
                MySqlDataReader reader = new MySqlCommand($"SELECT Accounts.username, GameType.GameName, COUNT(*), Place FROM GamePlayers Join Accounts ON GamePlayers.Accounts_ID = Accounts.id Join GameInstance on GameInstance.id = GamePlayers.GameInstance_ID Join GameType ON GameType.id = GameInstance.GameType_ID where Accounts.username = '{username}' and GameType.GameName = '{name}' and GamePlayers.Place != 1 GROUP BY Accounts.username, GameType.GameName, Place;", sqlController.connection).ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    output = reader["COUNT(*)"].ToString();
                }
                int num;
                int.TryParse(output, out num);
                Games a;
                Enum.TryParse(name, out a);
                stats.GameLosses.Add(a, num);
                reader.Close();
                reader.Dispose();
                Thread.Sleep(10);
            }

            //GamesPlayed
            foreach (var name in Enum.GetNames(typeof(Games)))
            {
                string output = "";
                MySqlDataReader reader = new MySqlCommand($"SELECT Accounts.username, GameType.GameName, COUNT(*) FROM GamePlayers Join Accounts ON GamePlayers.Accounts_ID = Accounts.id Join GameInstance on GameInstance.id = GamePlayers.GameInstance_ID Join GameType ON GameType.id = GameInstance.GameType_ID where Accounts.username = '{username}' and GameType.GameName = '{name}' GROUP BY Accounts.username, GameType.GameName;\r\n", sqlController.connection).ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    output = reader["COUNT(*)"].ToString();
                }
                int num;
                int.TryParse(output, out num);
                Games a;
                Enum.TryParse(name, out a);
                stats.GamesPlayed.Add(a, num);
                reader.Close();
                reader.Dispose();
                Thread.Sleep(10);
            }
            return stats;
        }
    }
}
