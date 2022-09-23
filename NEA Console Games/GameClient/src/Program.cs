using GameClient.src.menus;
using GameClient.src.networking;
using GameClient.src.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    class Program
    {
        //Entry point
        static void Main()
        {
            NetworkClient test = new NetworkClient("38.242.132.154", 5000);
            test.Connect();
            //Client.SetTitle($"Console Games [v{Client.Version}]");
            //MenuManager.Init();
            Console.ReadKey();
        }
    }
}
