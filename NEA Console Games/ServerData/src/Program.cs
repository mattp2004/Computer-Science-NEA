using ServerData.src.api;
using ServerData.src.data;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using ServerData.src.redis.server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataManager dataManager = new DataManager();
            dataManager.Run();
            Console.ReadKey();
        }
    }
}
