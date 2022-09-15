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
        public string GameName
        {
            get { return "Rock, Paper, Scissors"; }
        }
        public int MaxPlayers
        {
            get { return 2; }
        }
        public int RequiredPlayers
        {
            get { return 2; }
        }

        public RPS(Server server)
        {
            Players = new List<TcpClient>();
            serverInstance = server;
            rng = new Random();
        }

        Random rng;
        Server serverInstance;
        List<TcpClient> Players = new List<TcpClient>();

        public bool AddPlayer(TcpClient player)
        {
            if (!Players.Contains(player))
            {
                Players.Add(player);
                return true;
            }
            return false;
        }

        public void DisconnectClient(TcpClient client)
        {
            Players.Remove(client);
        }

        public void Start()
        {
            Server.SendMessageAll(Players, "WELCOME TO RPS");
        }
    }
}
