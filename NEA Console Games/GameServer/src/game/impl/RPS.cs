using GameServer.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            Server.SendMessageAll(Players, "Welcome to RPS");
            Thread.Sleep(15);
            for (int i = 0; i < Players.Count; i++)
            {
                Server.SendMessage(Players[i], $"You are player {i + 1}");
            }

            Dictionary<TcpClient, string> Responses = new Dictionary<TcpClient, string>();
            GameStatus = Status.RUNNING;

            //Gets player inputs
            Thread.Sleep(15);
            Responses = Server.RequestInputAll(Players, "Please pick either R/P/S").GetAwaiter().GetResult();
            Server.SendMessageAll(Players, $"PLAYER 1 CHOSE: {Responses[Players[0]]}");
            Server.SendMessageAll(Players, $"PLAYER 2 CHOSE: {Responses[Players[1]]}");
            if (Responses[Players[0]].ToLower() == "r")
            {
                if (Responses[Players[1]].ToLower() == "s")
                {
                    Server.SendMessageAll(Players, $"PLAYER 1 WON");
                }
                else if (Responses[Players[1]].ToLower() == "p")
                {
                    Server.SendMessageAll(Players, $"PLAYER 2 WON");
                }
                else
                {
                    Server.SendMessageAll(Players, $"IT'S A DRAW");
                }
            }
            else if (Responses[Players[0]].ToLower() == "s")
            {
                if (Responses[Players[1]].ToLower() == "p")
                {
                    Server.SendMessageAll(Players, $"PLAYER 1 WON");
                }
                else if (Responses[Players[1]].ToLower() == "r")
                {
                    Server.SendMessageAll(Players, $"PLAYER 2 WON");
                }
                else
                {
                    Server.SendMessageAll(Players, $"IT'S A DRAW");
                }
            }
            else if (Responses[Players[0]].ToLower() == "p")
            {
                if (Responses[Players[1]].ToLower() == "r")
                {
                    Server.SendMessageAll(Players, $"PLAYER 1 WON");
                }
                else if (Responses[Players[1]].ToLower() == "s")
                {
                    Server.SendMessageAll(Players, $"PLAYER 2 WON");
                }
                else
                {
                    Server.SendMessageAll(Players, $"IT'S A DRAW");
                }
            }
            Console.WriteLine("Game ending");
            Thread.Sleep(9500);
            for (int i = 0; i < Players.Count; i++)
            {
                if (!serverInstance.isDisconnected(Players[i]))
                    serverInstance.removeClient(Players[i]);
                else
                {
                    Console.WriteLine("Client disconnected from game.");
                }
            }
        }
    }
}
