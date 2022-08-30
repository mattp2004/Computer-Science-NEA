using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src
{
    class Client
    {
        public string ServerIP;
        public int Port;
        public bool Running { get; private set; }
        public TcpClient client;

        public Client(string ip, int port)
        {
            client = new TcpClient();
            Running = false;

            ServerIP = ip;
            Port = port;
        }

        public void Connect()
        {
            try
            {
                client.Connect(ServerIP, Port);
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error: ", e.Message);
            }
            if (client.Connected)
            {
                Console.WriteLine("Connected to the server.");
                Running = true;
            }
        }

        public void Run()
        {
            while (Running)
            {
                Console.ReadKey();
            }
        }
    }
}
