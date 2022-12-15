using Microsoft.Owin.Hosting;
using ServerData.src.api.Controllers.util;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerData.src.api
{
    public class API
    {
        public string port;
        public string Address;

        public static API instance;
        public API(string port)
        {
            instance = this;
            this.port = port;

            Address = $"http://*:{Config.port}/";
            //Address = $"http://localhost:{Config.port}/";
        }

        public void Run()
        {
            using (WebApp.Start<Config>(url: Address))
            {
                TextUtil.WriteLine($"\nAPI now Running on {Address}.", ConsoleColor.Cyan);
                Console.ReadLine();
            }
        }
    }
}
