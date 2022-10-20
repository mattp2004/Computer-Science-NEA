using GameServer.src.network;
using MySql.Data.MySqlClient.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.src.game.impl
{
    internal class Chatroom : IGame
    {
        public List<TcpClient> Clients { get; private set; }
        public Dictionary<TcpClient, string> ClientsDictionary { get; private set; }

        public Server instance;

        public Chatroom(Server inst)
        {
            instance = inst;
            Clients = new List<TcpClient>();
            ClientsDictionary = new Dictionary<TcpClient, string>();
        }
        //Properties
        #region properties
        public string GameName
        {
            get { return "Chatroom"; }
        }
        public int MaxPlayers
        {
            get { return 100; }
        }
        public int RequiredPlayers
        {
            get { return 2; }
        }
        #endregion

        public bool AddPlayer(TcpClient player)
        {
            Clients.Add(player);
            return true;
        }

        public void DisconnectClient(TcpClient client)
        {
            instance.SendPacket(client, new Packet("input", "off"));
            Clients.Remove(client);
        }

        public void Start()
        {
            Dictionary<TcpClient, bool> dict = new Dictionary<TcpClient, bool>();
            for(int i = 0; i < Clients.Count; i++)
            {
                dict[Clients[i]] = false;
                instance.SendPacket(Clients[i], new Packet("input", "on"));
            }
            List<Task> inputTasks = new List<Task>();
            while (true)
            {
                for(int i = 0; i < Clients.Count; i++)
                {
                    inputTasks.Add(GetInput(Clients[i]));
                }
                Task.WaitAll(inputTasks.ToArray(), 1000);
            }
        }

        public async Task GetInput(TcpClient client)
        {
            Packet a = instance.ReceivePacket(client).GetAwaiter().GetResult();
            Server.SendMessageAll(Clients, a.Content);
        }


    }
}