using GameServer.src.account;
using GameServer.src.config;
using GameServer.src.game;
using GameServer.src.game.impl;
using GameServer.src.misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerData;
using ServerData.src.redis;
using ServerData.src.redis.auth;

namespace GameServer.src.network
{
    enum Status
    {
        BOOTING,
        RUNNING,
        STOPPED
    }

    class Server
    {
        private static Server instance;
        private TcpListener listener;
        private IGame Game;
        private List<TcpClient> clients = new List<TcpClient>();
        private List<TcpClient> lobby = new List<TcpClient>();
        private Dictionary<TcpClient, IGame> ClientGame = new Dictionary<TcpClient, IGame>();
        private List<Thread> Games = new List<Thread>();
        private int Port;
        private string Name;
        public Random rng;
        private static bool inputting;

        public Status ServerStatus { get; private set; }

        public Server(string name, int port)
        {
            inputting = false;
            Game = new RPS(this);
            instance = this;
            this.Name = name;
            this.Port = port;
            rng = new Random();
            ServerStatus = Status.BOOTING;
            Util.Debug($"SERVER STATUS: {ServerStatus}");

            listener = new TcpListener(IPAddress.Any, port);
        }

        public void UpdateTitleStatus()
        {
            string TitleStatus = $"GameServer {Name} {ServerStatus} on {Port} with {clients.Count} and {lobby.Count} in Lobby with {Games.Count} games running.";
            Console.Title = TitleStatus;
        }

        public void Boot()
        {
            Console.WriteLine($"Game Server started on port {Port}");
            //Starts listener on specified port 
            listener.Start();
            ServerStatus = Status.RUNNING;
            Util.Debug($"SERVER STATUS: {ServerStatus}");
            RedisController t = new RedisController();
            AuthRepository authRepo = new AuthRepository(t);
            try
            {
                string uuid = authRepo.GetUUID("sfasfsa");
                Util.Debug($"Validated client {uuid}");
            }
            catch(Exception e) { }
            //Instantiates new list of Tasks to be run on the server
            List<Task> ConnectionTasks = new List<Task>();

            while (ServerStatus == Status.RUNNING)
            {
                if (!inputting) { ConsoleInput(); }
                UpdateTitleStatus();
                //Checks if there are any join requests on the listener
                if (listener.Pending())
                {
                    ConnectionTasks.Add(OnNewConnection());
                }
                if (lobby.Count >= Game.RequiredPlayers)
                {
                    Util.Debug($"New game starting '{Game.GameName}' with the following clients: ");
                    if (lobby.Count <= Game.MaxPlayers)
                    {
                        foreach (TcpClient c in lobby.ToArray())
                        {
                            Util.Debug($" - {c.Client.RemoteEndPoint}");
                            Game.AddPlayer(c);
                            lobby.Remove(c);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Game.MaxPlayers; i++)
                        {
                            Util.Debug($" - {lobby[i].Client.RemoteEndPoint}");
                            Game.AddPlayer(lobby[i]);
                        }
                    }                    
                    Thread gameThread = new Thread(new ThreadStart(Game.Start));
                    gameThread.Name = $"{Game.GameName}";
                    gameThread.Start();
                    Games.Add(gameThread);
                    UpdateTitleStatus();
                }
                for (int i = 0; i < lobby.Count; i++)
                {
                    bool disconnected = false;
                    TcpClient client = lobby[i];
                    disconnected = isDisconnected(client);
                    if (disconnected)
                    {
                        removeClient(client);
                    }
                    Thread.Sleep(10);
                }
            }
            Task.WaitAll(ConnectionTasks.ToArray(), 1000);
            listener.Stop();
            UpdateTitleStatus();

        }

        private async Task ConsoleInput()
        {
            inputting = true;
            await Task.Delay(1);
            Console.Write(">");
            string command = await Console.In.ReadLineAsync();
            ConsoleCommand(command);
            inputting = false;
        }

        private void ConsoleCommand(string command)
        {
            if (command == "help")
            {
                Console.WriteLine("HELPING");
            }
            else if (command == "games")
            {
                Console.WriteLine($"{Games[0].Name}");
            }
            else if(command == "debug")
            {
                Util.Write("Enabled debug mode.");
                Config.Debug = true;
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
            catch (SocketException)
            {
                return true;
            }
        }

        public void removeClient(TcpClient client)
        {
            Console.WriteLine($"{client.Client.RemoteEndPoint} has disconnected from us.");
            //Creates and sends a Disconnect Packet 
            Packet DisconnectPacket = new Packet("disconnect", "You have been disconnected from the server.");
            Task Disconnect = SendPacket(client, DisconnectPacket);
            //Removes client from the list of clients storing players
            clients.Remove(client);
            lobby.Remove(client);
            //Removes client from current game they may be in
            try
            {
                ClientGame[client].DisconnectClient(client);
            }
            catch (Exception e) { Util.Error(e); }
            Disconnect.GetAwaiter().GetResult();
            DestroyClient(client);
            //Closes client's network stream and client object.
        }

        public void DestroyClient(TcpClient client)
        {
            try
            {
                client.GetStream().Close();
                Util.Debug($"Destroyed client [{client.Client.RemoteEndPoint}]");
            }
            catch (Exception e) { Util.Error(e); }
            client.Close();
        }

        public async Task OnNewConnection()
        {
            //Accepts any pending connections 
            TcpClient newClient = await listener.AcceptTcpClientAsync();
            if (ValidateConnection(newClient))
            {
                Util.Debug($"New connection from {newClient.Client.RemoteEndPoint}");

                //Sends welcome message packet
                string msg = String.Format($"Welcome to the {Name} Game Server");
                Server.SendMessage(newClient, msg);

                //Adds client to the list of clients
                clients.Add(newClient);
                lobby.Add(newClient);
            }
        }

        private bool ValidateConnection(TcpClient newClient)
        {
            //Validates that the TCP Connection has been sent from the Client program
            string test = RequestInput(newClient, "auth").GetAwaiter().GetResult();
            RedisController t = new RedisController();
            AuthRepository authRepo = new AuthRepository(t);
            try
            {
                string uuid = authRepo.GetUUID("sfasfsa");
                Util.Debug($"Validated client {newClient.Client.RemoteEndPoint}");
                Console.WriteLine($"New client with uuid {uuid} connected from {newClient.Client.RemoteEndPoint}");
                return true;
            }
            catch(Exception e)
            {
                Util.Debug($"Failed to validate client {newClient.Client.RemoteEndPoint} | {e.Message}");
                return false; 
            }
        }

        public async Task SendPacket(TcpClient client, Packet packet)
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
                await client.GetStream().WriteAsync(Data, 0, Data.Length);
                Util.Debug($"Sent packet {packet.Type} {packet.Content} to client [{client.Client.RemoteEndPoint}]");
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message} | {e.TargetSite}");
                Util.Debug($"Failed to send packet {packet.Type} {packet.Content} to client [{client.Client.RemoteEndPoint}]");
            }
        }

        public Packet ReceiveNonAsyncPacket(TcpClient client)
        {
            Packet packet = null;
            try
            {
                //Creates empty packet type and gets the Network Data Stream from the client
                NetworkStream stream = client.GetStream();

                //Gets the length of the data buffer by taking the first 2 bytes (16 bits)
                byte[] bufferLength = new byte[2];
                stream.ReadAsync(bufferLength, 0, 2);
                //Converts bytes into an integer 
                int datalength = BitConverter.ToUInt16(bufferLength, 0);

                //Creates a new Byte array with the size of the data
                byte[] Data = new byte[datalength];
                stream.ReadAsync(Data, 0, Data.Length);

                //https://stackoverflow.com/questions/16072709/converting-string-to-byte-array-in-c-sharp
                //Converts the bytes into a string then deserializes it into a Packet object.
                string JsonString = Encoding.UTF8.GetString(Data);
                packet = Packet.Deserialize(JsonString);
            }
            catch(Exception e) { }
            return packet;
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
                Util.Debug($"Received packet {packet.Type} {packet.Content} from client [{client.Client.RemoteEndPoint}]");
                return packet;
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message} | {e.TargetSite}");
            }
            return null;
        }

        public static void SendMessageAll(List<TcpClient> clients, string msg)
        {
            Util.Debug($"Sent message {msg} to all clients");
            for (int i = 0; i < clients.Count; i++)
            {
                instance.SendPacket(clients[i], new Packet("msg", msg));
            }
        }

        public static void SendMessage(TcpClient client, string msg)
        {
            Util.Debug($"Sent message {msg} to all {client}");
            instance.SendPacket(client, new Packet("msg", msg));
        }

        public static async Task<string> RequestInput(TcpClient client, string msg)
        {
            string response = "";
            Packet RequestPacket = new Packet("input", msg);
            instance.SendPacket(client, RequestPacket);
            Util.Debug($"Requested input '{msg}' from [{client.Client.RemoteEndPoint}]");
            await Task.Delay(500);
            Packet Answer = instance.ReceivePacket(client).GetAwaiter().GetResult();
            response += Answer.Content;
            Util.Debug($"Recieved input '{response}' from [{client.Client.RemoteEndPoint}]");
            return response;
        }

        public static async Task<string> RequestAuth(TcpClient client, string msg)
        {
            string response = "";
            Packet RequestPacket = new Packet("auth", msg);
            instance.SendPacket(client, RequestPacket);
            Util.Debug($"Requested auth '{msg}' from [{client.Client.RemoteEndPoint}]");
            await Task.Delay(500);
            Packet Answer = instance.ReceivePacket(client).GetAwaiter().GetResult();
            response += Answer.Content;
            Util.Debug($"Recieved auth '{response}' from [{client.Client.RemoteEndPoint}]");
            return response;
        }

        public static async Task<Dictionary<TcpClient, string>> RequestInputAll(List<TcpClient> clients, string msg)
        {
            Util.Debug($"Requested input '{msg}' from all players");
            Dictionary<TcpClient, string> result = new Dictionary<TcpClient, string>();
            List<Task> InputTasks = new List<Task>();
            for (int i = 0; i < clients.Count; i++)
            {
                Console.WriteLine(i);
                InputTasks.Add(Server.RequestInput(clients[i], msg));
            }
            await Task.WhenAll(InputTasks);
            for (int i = 0; i < clients.Count; i++)
            {
                //https://briancaos.wordpress.com/2021/02/15/c-get-results-from-task-whenall/
                var fetchString = ((Task<string>)InputTasks[i]).Result;
                result.Add(clients[i], fetchString);
            }
            return result;
        }
    }
}
