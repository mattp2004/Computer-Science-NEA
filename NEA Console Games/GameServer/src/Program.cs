using GameServer.src.config;
using GameServer.src.misc;
using GameServer.src.network;
using ServerData.src.data;
using ServerData.src.redis;
using ServerData.src;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerData.src.redis.auth;
using GameServer.src.update;
using ServerData.src.api.Controllers;
using GameServer.src.data;

namespace GameServer
{
    class Program
    {
        static void Main()
        {
            ////Count of amount of times a place in a game occurs
            //SELECT Accounts.username, GameType.GameName, COUNT(*), Place FROM GamePlayers Join Accounts ON GamePlayers.Accounts_ID = Accounts.id Join GameInstance on GameInstance.id = GamePlayers.GameInstance_ID Join GameType ON GameType.id = GameInstance.GameType_ID where Accounts.username = 'matt' and GameType.GameName = 'BLACKJACK' GROUP BY Accounts.username, GameType.GameName, Place;

            ////Amount of times player won RPS
            //SELECT Accounts.username, GameType.GameName, COUNT(*), Place FROM GamePlayers Join Accounts ON GamePlayers.Accounts_ID = Accounts.id Join GameInstance on GameInstance.id = GamePlayers.GameInstance_ID Join GameType ON GameType.id = GameInstance.GameType_ID where Accounts.username = 'matt' and GameType.GameName = 'BLACKJACK' and GamePlayers.Place = 1 GROUP BY Accounts.username, GameType.GameName, Place;

            //SELECT* FROM Players Join Accounts ON Players.Accounts_ID = Accounts.id Join GameInstance on GameInstance.id = Players.GameInstance_ID Join GameType ON GameType.id = GameInstance.GameType_ID where Accounts.username = 'Bob' and GameType.GameName = 'RPS';



            //AMOUNT OF TIMES USER PLAYED THAT GAME
            //SELECT Accounts.username, GameType.GameName, COUNT(*) FROM Players Join Accounts ON Players.Accounts_ID = Accounts.id Join GameInstance on GameInstance.id = Players.GameInstance_ID Join GameType ON GameType.id = GameInstance.GameType_ID GROUP BY Accounts.username, GameType.GameName
            ////DataManager dataManager = new DataManager();
            BootUp();
            Server _server = new Server();
            _server.Boot();
            Console.ReadKey();
        }

        public static void BootUp()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Server booting please wait...");
            Thread.Sleep(350);
            if (!File.Exists("config.json"))
            {
                Config.GenerateConfig();
            }
            if (!File.Exists(Config.logsName))
            {
                Util.GenerateLogFolder();
            }
            UpdateManager.UpdateHash();
            Config.UpdateConfig();
            Thread.Sleep(250);
            Util.Write("Loading properties");
            Util.GenerateLog();

            Util.Write($"Starting {Config.serverName} at {DateTime.Now}");
            Util.Write($"Details:");
            Util.Write($"- IP: {Config.serverIP}");
            Util.Write($"- Port: {Config.serverPort}");
            Util.Write($"- DevServer: {Config.DevServer}");
            Util.Write($"- DefaultGame: {Config.DefaultGame}");
            Util.Write($"- Whitelisted: {Config.Whitelisted}");
            Util.Write($"- Version: {Config.version}");
            Thread.Sleep(800);

            Console.Title = Config.serverName;
        }
    }
}
