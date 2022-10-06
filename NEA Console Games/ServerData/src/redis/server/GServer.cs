using ServerData.src.redis.server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.data
{
    public class GServer
    {
        public int id;
        public string name;
        public int port;
        public string type;
        public int players, maxPlayers;
        public DateTime creationTime;
        public DateTime lastPing;

        public GServer(int id, string name, int port, string type, int players, int maxPlayers, DateTime creationTime, DateTime lastPing)
        {
            this.id = id;
            this.name = name;
            this.port = port;
            this.type = type;
            this.players = players;
            this.maxPlayers = maxPlayers;
            this.creationTime = creationTime;
        }

        public void Update(ServerRepository repo)
        {
            repo.PostServer(this);
        }
    }
}
