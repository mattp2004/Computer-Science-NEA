using ServerData.src.account;
using ServerData.src.api;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using ServerData.src.redis.server;
using ServerData.src.sql;
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
        public bool debug;
        public static DataManager instance;
        public RedisController redisController;
        public SQLController sqlController;
        public SqlRepository sqlRepository;
        public AccountRepository accountRepository;
        public AuthRepository authRepo;
        public ServerRepository serverRepo;
        public List<Thread> Tasks;
        public API api;

        public DataManager()
        {
            instance = this;

            debug = true;

            Tasks = new List<Thread>();
            redisController = new RedisController();
            sqlController = new SQLController();
            sqlRepository = new SqlRepository(sqlController);
            accountRepository = new AccountRepository(sqlRepository);
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
