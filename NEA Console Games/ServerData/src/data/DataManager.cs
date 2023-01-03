using ServerData.src.account;
using ServerData.src.api;
using ServerData.src.network;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using ServerData.src.redis.server;
using ServerData.src.sql;
using ServerData.src.sql.game;
using ServerData.src.stats;
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
        public SqlGameRepository sqlGameRepository;
        public StatsRepository statsRepo;

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
            sqlGameRepository = new SqlGameRepository(sqlController);
            statsRepo = new StatsRepository(sqlController);
        }

        public void Run()
        {
            sqlGameRepository.PopulateDataTables();
            Stats s = statsRepo.GetStatsFromUsername("matt");
            foreach(var g in s.GameWins)
            {
                Console.WriteLine("WINS: " + g.Key + ": " + g.Value);
            }
            foreach (var g in s.GameLosses)
            {
                Console.WriteLine("LOSSES: " + g.Key + ": " + g.Value);
            }
            foreach (var g in s.GamesPlayed)
            {
                Console.WriteLine("PLAYED: " + g.Key + ": " + g.Value);
            }
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
                serverRepo.UpdateServers();
                Thread.Sleep(240);
            }
        }
    }
}
