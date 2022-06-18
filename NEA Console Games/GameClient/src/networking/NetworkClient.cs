using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameClient.src.networking
{
    class NetworkClient
    {
        public TcpClient Client;
        public string ServerIP;
        public int ServerPort;

        public bool Running = false;

        public NetworkClient(string ip, int port)
        {
            ServerIP = ip;
            ServerPort = port;
            Client = new TcpClient();
        }

        public void ConnectToServer()
        {
            for(int i = 0; i < 3 && Client.Connected == false; i++)
            {
                try
                {
                    Client.Connect(ServerIP, ServerPort);
                }
                catch(Exception e)
                {
                    Util.Client.WriteLine($"(!) ERROR: {e.Message}\nFailed to connect to server x{i+1}.", ConsoleColor.Red);
                    Thread.Sleep(1500);
                }
            }
        }
    }
}
