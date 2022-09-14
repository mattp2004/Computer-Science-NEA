using GameServer.src.game;
using GameServer.src.game.impl;
using GameServer.src.network;
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


        public void Boot()
        {
            Console.WriteLine($"Game Server started on port {Port}");
            //Starts
            listener.Start();
            ServerStatus = Status.RUNNING;
            while (ServerStatus == Status.RUNNING)
            {
                //Checks if there are any join requests on the listener
                if (listener.Pending())
                {
                    OnNewConnection();
                }
            }
        }

        public async void OnNewConnection()
        {
            //Accepts any pending connections 
            TcpClient newClient = await listener.AcceptTcpClientAsync();
            Console.WriteLine($"New connection from {newClient.Client.RemoteEndPoint}.");

            //Sends welcome message packet
            string msg = String.Format($"Welcome to the {Name} Game Server");
            Packet t = new Packet("msg", "Welcome");
            SendPacket(newClient, t);

            //Adds client to the list of clients
            players.Add(newClient);
        }

        public async Task SendPacket(TcpClient client, Packet packet)
        {
            try
            {
                //Stores the json packet in a byte array buffer 
                byte[] jsonBuffer = Encoding.UTF8.GetBytes(packet.Serialize());
                //Stores the length of the jsonBuffer
                byte[] lengthBuffer = BitConverter.GetBytes(Convert.ToUInt16(jsonBuffer.Length));
                
                //Creates a new byte array with the size of the length and the data
                byte[] msgBuffer = new byte[lengthBuffer.Length + jsonBuffer.Length];
                //Copies the two arrays into the new array | {DataLength}{Data} 
                Array.Copy(lengthBuffer, msgBuffer, lengthBuffer.Length);
                Array.Copy(jsonBuffer,0, msgBuffer, lengthBuffer.Length, jsonBuffer.Length);

                //Writes data to the Network Stream
                await client.GetStream().WriteAsync(msgBuffer, 0, msgBuffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message} | {e.TargetSite}");
            }
        }

        public async Task<Packet> ReceivePacket(TcpClient client)
        {
            try
            {
                //Creates empty packet type and gets the Network Data Stream from the client
                Packet packet;
                NetworkStream stream = client.GetStream();

                //Gets the length of the data buffer by taking the first 2 bytes (16 bits)
                byte[] bufferLength = new byte[2];
                await stream.ReadAsync(bufferLength, 0, 2);
                //Converts bytes into an integer 
                int datalength = BitConverter.ToUInt16(bufferLength, 0);

                //Creates a new Byte array with the size of the data
                byte[] Data = new byte[datalength];
                await stream.ReadAsync(Data, 0, Data.Length);

                //https://stackoverflow.com/questions/16072709/converting-string-to-byte-array-in-c-sharp
                //Converts the bytes into a string then deserializes it into a Packet object.
                string JsonString = Encoding.UTF8.GetString(Data);
                packet = Packet.Deserialize(JsonString);

                //Returns newly created Packet.
                return packet;
            }
            catch(Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message} | {e.TargetSite}");
            }
            return null;
        }

    }
}
