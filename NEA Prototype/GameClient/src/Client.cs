using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameClient.src
{
    class Client
    {
        public string ServerIP;
        public int Port;
        public TcpClient client;
        public NetworkStream stream;
        public Status ClientStatus;
        public List<Task> TaskList;

        public enum Status
        {
            CONNECTING,
            CONNECTED,
            DISCONNECTED
        }

        public Client(string ip, int port)
        {
            client = new TcpClient();
            ClientStatus = Status.DISCONNECTED;
            ServerIP = ip;
            Port = port;
        }

        public void Connect()
        {
            ClientStatus = Status.CONNECTING;
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
                ClientStatus = Status.CONNECTED;
                stream = client.GetStream();
                Console.WriteLine("Connected to the server.");
                Run();
            }
        }

        public void Run()
        {
            TaskList = new List<Task>();
            while (ClientStatus == Status.CONNECTED)
            {
                TaskList.Add(ReceivePacket());
                Thread.Sleep(10);

            }
            Task.WaitAll(TaskList.ToArray(), 1000); ;
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
                    Console.Write(packet.Content);
                }
                
            }
            catch(Exception e)
            {

            }
            
        }

        public async Task SendPacket(Packet packet)
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
                Array.Copy(jsonBuffer, 0, msgBuffer, lengthBuffer.Length, jsonBuffer.Length);

                //Writes data to the Network Stream
                await stream.WriteAsync(msgBuffer, 0, msgBuffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message} | {e.TargetSite}");
            }
        }
    }
}
