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
            Clients.Remove(client);
        }

        public void Start()
        {
            Dictionary<TcpClient, bool> dict = new Dictionary<TcpClient, bool>();
            for(int i = 0; i < Clients.Count; i++)
            {
                dict[Clients[i]] = false;
            }
            while (true)
            {
                for(int i = 0; i < Clients.Count; i++)
                {
                    if (dict[Clients[i]] == false)
                    {
                        Packet a = new Packet("input", "Enter message:");
                        instance.SendPacket(Clients[i], a);
                        dict[Clients[i]] = true;
                    }
                    Packet t = instance.ReceiveNonAsyncPacket(Clients[i]);
                    if (t == null || t.Type != "input"){
                        continue;
                    }
                    else
                    {
                        dict[Clients[i]] = false;
                        Server.SendMessageAll(Clients, t.Content);
                    }
                }
            }
        }
    }
}