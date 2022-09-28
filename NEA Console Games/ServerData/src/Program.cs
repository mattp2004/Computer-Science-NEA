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
            GameServer gameServer = new GameServer();
            gameServer.port = 6000;
            gameServer.name = "GameServer";
            gameServer.lastPing = DateTime.Now;
            gameServer.creationTime = DateTime.Now;
            gameServer.players = 1;
            gameServer.maxPlayers = 50;
            gameServer.id = 1;
            RedisController redisController = new RedisController();
            ServerRepository repo = new ServerRepository(redisController);
            repo.PostServer(gameServer);
            Console.ReadKey();
            Auth auth = new Auth("c1a0e30d-9b68tetst4e97-825f-sa", "sfasfsa");
            AuthRepository authRepo = new AuthRepository(redisController);
            authRepo.PostAuth(auth);
            Console.ReadKey();
        }
    }
}
