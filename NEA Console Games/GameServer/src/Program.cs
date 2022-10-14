using GameServer.src.account;
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

namespace GameServer
{
    class Program
    {
        static void Main()
        {
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
