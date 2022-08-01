using GameServer.src.misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.src.update
{
    public static class UpdateManager
    {
        private static object process;

        public static int Hash { get; set; }

        public static bool CheckUpdated()
        {
            FileInfo fileInfo = new FileInfo(config.Config.fileName);
            return (fileInfo.GetHashCode() != Hash);
        }

        public static void UpdateHas()
        {
            FileInfo fi = new FileInfo(config.Config.fileName);
        }

        public static void RestartInstance()
        {
            Util.Log("[Update found] Restarting instance");
            Console.WriteLine("Instance restarting for an update");
            Thread.Sleep(1500);
            Process.Start(config.Config.fileName);
            Thread.Sleep(250);
            Environment.Exit(0);
        }
    }
}
