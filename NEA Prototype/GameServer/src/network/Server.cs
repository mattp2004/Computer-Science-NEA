using GameServer.src.game;
using GameServer.src.game.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src
{

    enum Status
    {
        BOOTING,
        RUNNING,
        STOPPED
    }

    class Server
    {

        private TcpListener listener;
        private IGame nextGame;
        private List<TcpClient> players = new List<TcpClient>();
        private int Port;
        private string Name;
        public Random rng = new Random();

        public Status ServerStatus { get; private set; }

        public Server(string name, int port)
        {
            nextGame = new RPS();

            this.Name = name;
            this.Port = port;
            ServerStatus = Status.BOOTING;

            listener = new TcpListener(IPAddress.Any, port);
        }


        public void Run()
        {
            Console.WriteLine("Game Server started on port 8000");
            listener.Start();
            ServerStatus = Status.RUNNING;
            while (ServerStatus == Status.RUNNING)
            {
                if (listener.Pending())
                {
                    OnNewConnection();
                }
            }
        }

        public async void OnNewConnection()
        {
            TcpClient newClient = await listener.AcceptTcpClientAsync();
            Console.WriteLine($"New connection from {newClient.Client.RemoteEndPoint}.");
        }

    }
}
