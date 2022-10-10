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
using GameClient.src.api;
using Newtonsoft.Json;
using ServerData.src.api;

namespace GameClient
{
    class Program
    {
        class tokena
        {
            public string TOKEN;
        }
        //Entry point
        static void Main()
        {
            ApiController apiController = new ApiController();
            ApiRepository apiRepo = new ApiRepository(apiController);
            apiRepo.SetAuth(apiRepo.GenerateUUID(), apiRepo.GenerateToken());

            Console.ReadKey();
            Client.SetTitle($"Console Games [v{Client.Version}]");
            MenuManager.Init();
            Console.ReadKey();
        }
    }
}
