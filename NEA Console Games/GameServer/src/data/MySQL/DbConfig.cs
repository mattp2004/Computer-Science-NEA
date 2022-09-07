using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.data.MySQL
{
    static class DbConfig
    {
        public const string HOSTNAME = "localhost";
        public const string USERNAME = "GameServer";
        public const string PASSWORD = "YZ3L0AYN5cMCSOQZ";
        public static string DATABASE = config.Config.DevServer ? "data_dev" : "data";
    }
}
