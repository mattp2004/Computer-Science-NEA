using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Memcached;
using ServerData.src.data;
using ServerData.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerData.src.sql.game
{
    public class SqlGameRepository
    {
        SQLController sqlController;
        public SqlGameRepository(SQLController sqlController)
        {
            this.sqlController = sqlController;
        }

        public void PopulateDataTables()
        {
            string query = "TRUNCATE `GameType`;";
            MySqlCommand a = new MySqlCommand(query, sqlController.connection);
            a.ExecuteNonQuery();
            Thread.Sleep(100);
            foreach (string name in Enum.GetNames(typeof(Games)))
            {
                Console.WriteLine(name);
                string query2 = $"INSERT INTO GameType VALUES(NULL,'{name}');";
                MySqlCommand b = new MySqlCommand(query2, sqlController.connection);
                b.ExecuteNonQuery();
                Thread.Sleep(5);
            }
        }

        public void PostGame(Games game, Dictionary<string,string> clients, long startTime, long endTime)
        {
            string gameID = "a";
            MySqlDataReader reader = new MySqlCommand($"SELECT ID FROM GameType WHERE GameName = '{Enum.GetName(typeof(Games), game)}'", sqlController.connection).ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                gameID = reader["ID"].ToString();
            }
            Console.WriteLine("GAME" + gameID);
            reader.Close();
            reader.Dispose(); 
            string GameInstanceID = "a";
            MySqlDataReader reader2 = new MySqlCommand($"INSERT INTO GameInstance VALUES (NULL, {gameID}); SELECT LAST_INSERT_ID();", sqlController.connection).ExecuteReader();
            reader2.Read();
            if (reader2.HasRows)
            {
                GameInstanceID = reader2["LAST_INSERT_ID()"].ToString();
            }
            Console.WriteLine("GAME" + GameInstanceID);
            reader2.Close();
            reader2.Dispose();

            string query = $"INSERT INTO GameStats VALUES (NULL, {startTime}, {endTime}, {clients.Count}, {GameInstanceID});";
            MySqlCommand c = new MySqlCommand(query, sqlController.connection);
            c.ExecuteNonQuery();

            foreach(var g in clients)
            {
                string q = $"INSERT INTO GamePlayers VALUES (NULL, {GameInstanceID}, {g.Key}, {g.Value});";
                MySqlCommand sqlCommand = new MySqlCommand(q, sqlController.connection);
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
