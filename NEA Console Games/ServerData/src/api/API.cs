using Microsoft.Owin.Hosting;
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

        private List<Thread> Tasks;

        public static API instance;
        public API(string port)
        {
            instance = this;
            this.port = port;

            Address = $"http://*:{Config.port}/";
            //Address = $"http://localhost:{Config.port}/";
            Tasks = new List<Thread>();
        }

        public void Run()
        {
            Thread DataTask = new Thread(new ThreadStart(UpdateData));
            DataTask.Start();
            Tasks.Add(DataTask);
            using (WebApp.Start<Config>(url: Address))
            {
                Console.WriteLine($"API Running on {Address}");
                Console.ReadLine();
            }
        }
    }
}
