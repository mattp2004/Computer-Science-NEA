using GameServer.src.config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.src.network
{
    enum ServerStatus
    {
        RUNNING,
        CLOSED
    }

    class Server
    {
        private TcpListener listener;

        private string IP;
        private int Port;
        private string Name;
        private ServerStatus Status;

        public Server()
        {
            IP = Config.serverIP;
            Port = Config.serverPort;
            Name = Config.serverName;
            listener = new TcpListener(IPAddress.Any, Port);
        }

        public void Run()
        {
            Console.WriteLine($"Starting {Name} Game Server.\n- IP: {Config.serverIP}\n- Port: {Config.serverPort}\n- Status: ALIVE");
            Console.WriteLine("Press Ctrl-C to shutdown the server.");
            listener.Start();
            Status = ServerStatus.RUNNING;

            List<Task> joinTasks = new List<Task>();

            while (Status == ServerStatus.RUNNING)
            {
                if (listener.Pending())
                {
                    joinTasks.Add(AcceptConnection());
                }
            }
        }

        public async Task AcceptConnection()
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine($"New connection from [{client.Client.RemoteEndPoint}] -> [0.0.0.0]");
        }
    }
}
