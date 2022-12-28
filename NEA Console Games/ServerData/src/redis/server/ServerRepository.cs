using ServerData.src.data;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerData.src.redis.server
{
    public class ServerRepository
    {
        private RedisController redisController { get; }
        public long ServerExpiry = 60000;

        private List<GServer> servers;

        public ServerRepository(RedisController redisController)
        {
            this.redisController = redisController;
            servers = new List<GServer>();
        }
        
        public GServer GetServer(string name)
        {
            HashEntry[] Hashes = redisController.database.HashGetAll(name);
            GServer t = new GServer(int.Parse(Hashes[0].Value), Hashes[1].Value, int.Parse(Hashes[2].Value), "GameServer", int.Parse(Hashes[3].Value), int.Parse(Hashes[4].Value), DateTime.Parse(Hashes[5].Value), DateTime.Parse(Hashes[6].Value));
            return t;
        }

        public void PostServer(GServer server)
        {
            string name = server.name + "-" + server.id;
            if (!servers.Contains(server))
            {
                while (redisController.database.KeyExists(name))
                {
                    server.id += 1;
                    name = server.name + "-" + server.id;
                }
            }
            HashEntry[] Hashes = new HashEntry[7];
            Hashes[0] = new HashEntry("id",server.id);
            Hashes[1] = new HashEntry("name", server.name);
            Hashes[2] = new HashEntry("port", server.port);
            Hashes[3] = new HashEntry("players", server.players);
            Hashes[4] = new HashEntry("maxPlayers", server.maxPlayers);
            Hashes[5] = new HashEntry("created", server.creationTime.ToString());
            Hashes[6] = new HashEntry("lastPing", server.lastPing.ToString());

            redisController.database.HashSet(name, Hashes);
            servers.Add(server);
        }

        public void DeleteServer(GServer server)
        {
            redisController.database.KeyDelete(server.name + "-" + server.id);
        }

        public void DeleteServer(string serverName)
        {
            redisController.database.KeyDelete(serverName);
        }

        public void UpdateServers()
        {
            int id = 1;
            string name = "GameServer" + "-" + id;
            while (redisController.database.KeyExists(name))
            {
                GServer a = GetServer(name);
                if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - a.lastPing.Millisecond < ServerExpiry)
                {
                    DeleteServer(a);
                }
                id += 1;
                name = "GameServer" + "-" + id;
            }
        }
    }
}
