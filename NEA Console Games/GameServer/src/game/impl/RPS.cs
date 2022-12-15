using GameServer.src.misc;
using GameServer.src.network;
using ServerData.src.data;
using ServerData.src.network;
using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Threading;

namespace GameServer.src.game.impl
{
    class RPS : IGame
    {
        Random rng;
        Server serverInstance;
        List<Client> Players = new List<Client>();
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
            Players = new List<Client>();
            serverInstance = server;
            rng = new Random();
            GameStatus = Status.STOPPED;
        }

        public bool AddPlayer(Client player)
        {
            if (!Players.Contains(player))
            {
                Players.Add(player);
                return true;
            }
            return false;
        }

        public void DisconnectClient(Client client)
        {
            Players.Remove(client);
        }

        public void Start()
        {
            Server.SendMessageAll(Players, "Welcome to RPS");
            Thread.Sleep(15);

            Dictionary<Client, string> Responses = new Dictionary<Client, string>();
            GameStatus = Status.RUNNING;

            //Gets player inputs
            Thread.Sleep(15);
            Responses = Server.RequestInputAll(Players, "Please pick either R/P/S").GetAwaiter().GetResult();
            Server.SendMessageAll(Players, $"{Players[0].GetAccount().GetUsername()} CHOSE: {Responses[Players[0]]}");
            Thread.Sleep(40);
            Server.SendMessageAll(Players, $"{Players[1].GetAccount().GetUsername()} CHOSE: {Responses[Players[1]]}");
            if (Responses[Players[0]].ToLower() == "r")
            {
                if (Responses[Players[1]].ToLower() == "s")
                {
                    Server.SendMessageAll(Players, $"{Players[0].GetAccount().GetUsername()} WON");
                }
                else if (Responses[Players[1]].ToLower() == "p")
                {
                    Server.SendMessageAll(Players, $"{Players[1].GetAccount().GetUsername()} WON");
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
                    Server.SendMessageAll(Players, $"{Players[0].GetAccount().GetUsername()} WON");
                }
                else if (Responses[Players[1]].ToLower() == "r")
                {
                    Server.SendMessageAll(Players, $"{Players[1].GetAccount().GetUsername()} WON");
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
                    Server.SendMessageAll(Players, $"{Players[0].GetAccount().GetUsername()} WON");
                }
                else if (Responses[Players[1]].ToLower() == "s")
                {
                    Server.SendMessageAll(Players, $"{Players[1].GetAccount().GetUsername()} WON");
                }
                else
                {
                    Server.SendMessageAll(Players, $"IT'S A DRAW");
                }
            }
            serverInstance.accountRepository.GiveTokens(Players[0].GetAccount(), 250);
            serverInstance.accountRepository.GiveTokens(Players[1].GetAccount(), 250);
            Server.SendMessageAll(Players, "You have been awarded 250 tokens for playing!");
            Console.WriteLine("Game ending");
            serverInstance.Queue[ServerData.src.data.Games.RPS].Clear();
            serverInstance.GameTypes.Remove(Games.RPS);
            serverInstance.GameTypes.Add(Games.RPS, new RPS(serverInstance));
            Thread.Sleep(9500);
            serverInstance.DisconnectClient(Players[0], "Game ended");
            Thread.Sleep(150);
            serverInstance.DisconnectClient(Players[1], "Game ended");
            Thread.Sleep(1000);
        }
    }
}
