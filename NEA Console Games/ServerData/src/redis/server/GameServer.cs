using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.data
{
    public class GameServer
    {
        public int id;
        public string name;
        public int port;
        public DateTime creationTime;
        public int players, maxPlayers;
        public DateTime lastPing;

        public GameServer()
        {

        }
    }
}
