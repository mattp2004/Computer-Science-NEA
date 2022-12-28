using GameClient.src.api;
using ServerData.src.account;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using ServerData.src.redis.server;
using ServerData.src.sql;
using ServerData.src.sql.game;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.data
{
    public class DataManager
    {
        public static DataManager instance;

        public RedisController redisController;
        public SQLController sqlController;

        public SqlRepository sqlRepository;
        public ServerRepository serverRepository;
        public AccountRepository accountRepository;
        public AuthRepository authRepository;

        public SqlGameRepository sqlGameRepository;

        public DataManager()
        {
            instance = this;

            redisController = new RedisController();
            sqlController = new SQLController();

            sqlRepository = new SqlRepository(sqlController);
            serverRepository = new ServerRepository(redisController);
            accountRepository = new AccountRepository(sqlRepository);

            sqlGameRepository = new SqlGameRepository(sqlController);
        }
    }
}
