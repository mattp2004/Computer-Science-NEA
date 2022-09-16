using GameServer.src.network;
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
        Random rng;
        Server serverInstance;
        List<TcpClient> Players = new List<TcpClient>();
        Status GameStatus;
        //Properties
        #region properties
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
        #endregion

        public RPS(Server server)
        {
            Players = new List<TcpClient>();
            serverInstance = server;
            rng = new Random();
            GameStatus = Status.STOPPED;
        }

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
            Dictionary<TcpClient, string> Responses = new Dictionary<TcpClient, string>();
            GameStatus = Status.RUNNING;
            Responses = Server.RequestInputAll(Players, "Please pick either R/P/S").GetAwaiter().GetResult();
            //Console.WriteLine(Server.RequestInput(Players[1], "Please pick r/p/s"));
            //for(int i = 0; i < Players.Count; i++)
            //{
            //    Responses[Players[i]] = Server.RequestInput(Players[i], "enter rps").GetAwaiter().GetResult();
            //}
            Server.SendMessageAll(Players, $"PLAYER 1 CHOSE: ${Responses[Players[0]]}");
            Server.SendMessageAll(Players, $"PLAYER 2 CHOSE: ${Responses[Players[1]]}");
        }
    }
}
