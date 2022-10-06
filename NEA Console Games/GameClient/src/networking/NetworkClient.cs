using GameClient.src.Util;
using ServerData.src.redis;
using ServerData.src.redis.auth;
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
        public string ServerIP;
        public int Port;
        public TcpClient client;
        public NetworkStream stream;
        public Status ClientStatus;
        public List<Task> TaskList;
        public string Token;
        public string Uuid;

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
            Token = ServerData.src.data.DataUtil.GenerateToken();
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
        }

        public void Disconnect()
        {
            SendPacket(new Packet("disconnect", ""));
            //Closes the Network Stream and the TcpClient connection
            ClientStatus = Status.DISCONNECTED;
            stream.Close();
            client.Close();
        }

        public void Run()
        {
            TaskList = new List<Task>();
            while (ClientStatus == Status.CONNECTED)
            {
                //Checking for new packets being sent from the server every 10ms
                TaskList.Add(ReceivePacket());
                Thread.Sleep(10);
                if (isDisconnected(client))
                {
                    Console.WriteLine("Disconnected from server.");
                    ClientStatus = Status.DISCONNECTED;
                }
            }
            Task.WaitAll(TaskList.ToArray(), 1000); Console.ReadLine();
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

                    //Dictionary<string, Task> PacketTasks = new Dictionary<string, Task>();
                    //PacketTasks.Add("msg", PacketMessage(packet));
                    //PacketTasks.Add("input", PacketInput(packet));
                    //PacketTasks.Add("auth", PacketAuth(packet));
                    //PacketTasks.Add("disconnect", PacketDisconnect(packet));

                    //Console.Write(PacketTasks[packet.Type]);
                    //PacketTasks[packet.Type].Start();
                    //PacketTasks[packet.Type].GetAwaiter().GetResult();
                    if (packet.Type == "msg")
                    {
                        Console.WriteLine(packet.Content);
                    }
                    if (packet.Type == "input")
                    {
                        //Takes in an input
                        Console.WriteLine(packet.Content);
                        Console.WriteLine(">");

                        string Response = Console.ReadLine();
                        //Returns that input to the server through a Response Packet
                        Packet resp = new Packet("input", Response);
                        await SendPacket(resp);
                    }
                    if (packet.Type == "auth")
                    {
                        //Takes in an input
                        Console.WriteLine(packet.Content);
                        Console.WriteLine(">");

                        string Response = Console.ReadLine();
                        //Returns that input to the server through a Response Packet
                        Packet resp = new Packet("input", Response);
                        await SendPacket(resp);
                    }
                    if (packet.Type == "disconnect")
                    {
                        //Disconnects the Client after having recieved a disconnect packet.
                        Console.WriteLine(packet.Content);
                        ClientStatus = Status.DISCONNECTED;
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

        public async Task PacketAuth(Packet packet)
        {
            Packet resp = new Packet("auth", Token);
            await SendPacket(resp);
        }

        public async Task PacketDisconnect(Packet packet)
        {
            //Disconnects the Client after having recieved a disconnect packet.
            Client.WriteLine(packet.Content);
            Disconnect();
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
    }
}
