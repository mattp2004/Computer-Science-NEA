using GameServer.src.account;
using GameServer.src.config;
using GameServer.src.misc;
using GameServer.src.network;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        //public static OdbcConnection conn = new System.Data.Odbc.OdbcConnection("DRIVER={MySQL ODBC 5.3 ANSI Driver};SERVER=localhost;PORT=3306;DATABASE=data;USER=GameServer;PASSWORD=YZ3L0AYN5cMCSOQZ;OPTION=3;");
        static void Main()
        {
            //OdbcConnection conn = new System.Data.Odbc.OdbcConnection("DRIVER={MySQL ODBC 5.3 ANSI Driver};SERVER=localhost;PORT=3306;DATABASE=data;USER=root;PASSWORD=root;OPTION=3;");
            //conn.Open();
            //Client temp = new Client(new TcpClient(), "test");
            //Account a = AccountManager.FetchAccount(temp);
            //Console.WriteLine(a.GetName());
            BootUp();
            Server _server = new Server();
            _server.Run();
            Console.ReadKey();
        }

        public static void BootUp()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Server booting. This could take up to 5 seconds in order to make sure other instances have been killed.");
            Thread.Sleep(350);
            if (!File.Exists("config.json"))
            {
                Config.GenerateConfig();
            }
            if (!File.Exists(Config.logsName))
            {
                Util.GenerateLogFolder();
            }

            Config.UpdateConfig();
            Util.GenerateLog();

            Util.Log($"Started {Config.serverName} at {DateTime.Now}");
            Util.Log($"Details:");
            Util.Log($"- IP: {Config.serverIP}");
            Util.Log($"- Port: {Config.serverPort}");
            Util.Log($"- DevServer: {Config.DevServer}");
            Util.Log($"- DefaultGame: {Config.DefaultGame}");
            Util.Log($"- Whitelisted: {Config.Whitelisted}");
            Util.Log($"- Version: {Config.version}");

            Console.Title = Config.serverName;
        }
    }
}
