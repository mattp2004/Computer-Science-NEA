using ServerData.src.api;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using ServerData.src.redis.server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerData.src.data
{
    class DataManager
    {
        public static DataManager instance;
        public RedisController redisController;
        public AuthRepository authRepo;
        public ServerRepository serverRepo;
        public List<Thread> Tasks;
        public API api;

        public DataManager()
        {
            instance = this;

            Tasks = new List<Thread>();
            redisController = new RedisController();
            authRepo = new AuthRepository(redisController);
            serverRepo = new ServerRepository(redisController);
            api = new API(Config.port);
        }

        public void Run()
        {
            Thread updater = new Thread(UpdateData);
            updater.Start();
            Tasks.Add(updater);

            api.Run();
        }

        public void UpdateData()
        {
            while (true)
            {
                authRepo.UpdateKeys();
                Thread.Sleep(240);
            }
        }
    }
}
