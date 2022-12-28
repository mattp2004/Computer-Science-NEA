using GameClient.src.data;
using GameClient.src.Util;
using ServerData.src.network;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client = GameClient.src.Util.Client;

namespace GameClient.src.networking
{
    class NetworkClient
    {
        public string ServerIP;
        public int Port;
        public TcpClient client;
        public NetworkStream stream;
        public Status ClientStatus;
        public List<Task> TaskList;
        public string Uuid;
        public bool PreviousConnection;
        public bool RequestDisconnect;

        public enum Status
        {
            CONNECTING,
            CONNECTED,
            DISCONNECTED
        }

        public NetworkClient(string ip, int port)
        {
            client = new TcpClient();
            ClientStatus = Status.DISCONNECTED;
            ServerIP = ip;
            Port = port;
            Uuid = ServerData.src.data.DataUtil.GenerateUUID();
        }

        public void Connect()
        {
            ClientStatus = Status.CONNECTING;
            try
            {
                //Connect client to the ip and port of the server's TCP Listener.
                client.Connect(ServerIP, Port);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: ", e.Message);
            }
            if (client.Connected)
            {
                ClientStatus = Status.CONNECTED;
                stream = client.GetStream();
                Console.WriteLine("Connected to the server.");
                Run();
            }
            else
            {
                _cleanupNetworkResources();
                Console.WriteLine("Unable to connect to server");
            }
        }

        public async void Disconnect()
        {
            await SendPacket(new Packet("disconnect", ""));
            //Closes the Network Stream and the TcpClient connection
            ClientStatus = Status.DISCONNECTED;
            RequestDisconnect = true;
        }

        public void HandleDisconnect(Packet p)
        {
            ClientStatus = Status.DISCONNECTED;
            Console.Write(p.Content);
            Thread.Sleep(1500);
        }

        public bool isDisconnected(TcpClient client)
        {
            //Checks if the TcpClient has been disconnected
            //https://stackoverflow.com/questions/722240/instantly-detect-client-disconnection-from-server-socket
            try
            {
                Socket clientSocket = client.Client;
                return clientSocket.Poll(1, SelectMode.SelectRead) && clientSocket.Available == 0;
            }
            catch (SocketException) { return true; }
        }

        public void Run()
        {
            TaskList = new List<Task>();
            PreviousConnection = true;
            while (ClientStatus == Status.CONNECTED)
            {
                //Checking for new packets being sent from the server every 10ms
                TaskList.Add(ReceivePacket());
                Thread.Sleep(10);
                if (isDisconnected(client) && !RequestDisconnect)
                {
                    Console.WriteLine("Server has disconnected from us.");
                    ClientStatus = Status.DISCONNECTED;
                }
            }
            Task.WaitAll(TaskList.ToArray(), 1000);
            _cleanupNetworkResources();
            if (PreviousConnection)
            {
                Console.WriteLine("Disconnected.");
            }
        }

        public async Task ReceivePacket()
        {
            try
            {
                if (client.Available > 0)
                {

                    //Creates empty packet type
                    Packet packet;

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
                    if (packet.Type == "msg")
                    {
                        Client.WriteLine(packet.Content);
                        //PacketMessage(packet).GetAwaiter().GetResult();
                    }
                    if (packet.Type == "input")
                    {
                        PacketInput(packet).GetAwaiter().GetResult();
                    }
                    if(packet.Type == "title")
                    {
                        if (packet.Content == "reset")
                        {
                            Client.SetTitle(Client.Title);
                        }
                        Client.SetTitle(packet.Content);
                    }
                    if (packet.Type == "auth")
                    {
                        PacketAuth().GetAwaiter().GetResult();
                    }
                    if (packet.Type == "game")
                    {
                        GameSelect().GetAwaiter().GetResult();
                    }
                    if (packet.Type == "disconnect")
                    {
                        //Disconnects the Client after having recieved a disconnect packet.
                        HandleDisconnect(packet);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message} | {e.TargetSite}");
            }

        }

        public async Task PacketMessage(Packet packet)
        {
            Client.WriteLine(packet.Content);
        }
        public async Task PacketInput(Packet packet)
        {
            //Takes in an input
            Client.WriteLine(packet.Content);
            Client.WriteLine(">");

            string Response = Client.GetInput();
            //Returns that input to the server through a Response Packet
            Packet resp = new Packet("input", Response);
            await SendPacket(resp);
        }

        public async Task PacketAuth()
        {
            Packet resp = new Packet("auth", DataManager.GetInstance().accessToken.Substring(1, DataManager.GetInstance().accessToken.Length - 2));
            Console.WriteLine("Sent packet: " + resp.Type + " " + resp.Content);
            await SendPacket(resp);
        }
        public async Task GameSelect()
        {
            Packet resp = new Packet("game", DataManager.GetInstance().GameSelected.ToString());
            Console.WriteLine("Sent packet: " + resp.Type + " " + resp.Content);
            await SendPacket(resp);
        }

        public async Task PacketDisconnect(Packet packet)
        {
            //Disconnects the Client after having recieved a disconnect packet.
            Client.WriteLine(packet.Content);
            Disconnect();
        }

        private void _cleanupNetworkResources()
        {
            stream?.Close();
            stream = null;
            client.Close();
        }

        public async Task SendPacket(Packet packet)
        {
            try
            {
                //Stores the json packet in a byte array buffer 
                byte[] packetBuffer = Encoding.UTF8.GetBytes(packet.Serialize());
                //Stores the length of the jsonBuffer
                byte[] bufferLength = BitConverter.GetBytes(Convert.ToUInt16(packetBuffer.Length));

                //Creates a new byte array with the size of the length and the data
                byte[] Data = new byte[bufferLength.Length + packetBuffer.Length];
                //Copies the two arrays into the new array | {DataLength}{Data} 
                Array.Copy(bufferLength, Data, bufferLength.Length);
                Array.Copy(packetBuffer, 0, Data, bufferLength.Length, packetBuffer.Length);

                //Writes data to the Network Stream
                await stream.WriteAsync(Data, 0, Data.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message} | {e.TargetSite}");
            }
        }
    }
}
