using GameClient.src.menus;
using GameClient.src.networking;
using GameClient.src.Util;
using ServerData.src.redis.auth;
using ServerData.src.redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerData.src.data;

namespace GameClient
{
    class Program
    {
        //Entry point
        static void Main()
        {
            Console.ReadKey();
            Client.SetTitle($"Console Games [v{Client.Version}]");
            MenuManager.Init();
            Console.ReadKey();
        }
    }
}
