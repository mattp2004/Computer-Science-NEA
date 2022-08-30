using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.game.impl
{
    class RPS : IGame
    {
        public string Name => throw new NotImplementedException();

        public int RequiredPlayers => throw new NotImplementedException();

        public int MaxPlayers => throw new NotImplementedException();

        public bool AddPlayer(TcpClient player)
        {
            throw new NotImplementedException();
        }

        public void DisconnectClient(TcpClient client)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}
