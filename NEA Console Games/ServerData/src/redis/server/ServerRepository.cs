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
        private List<GServer> servers;

        public ServerRepository(RedisController redisController)
        {
            this.redisController = redisController;
            servers = new List<GServer>();
        }

        public void PostServer(GServer server)
        {
            string name = server.name + "-" + server.id;
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
    }
}
