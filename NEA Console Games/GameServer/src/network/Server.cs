using GameServer.src.config;
using GameServer.src.game;
using GameServer.src.game.impl;
using GameServer.src.misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using GameServer.src.update;
using ServerData.src.data;
using ServerData.src.redis.server;
using ServerData.src.network;
using ServerData.src.account;
using ServerData.src.sql;
using GameServer.src.data;

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
        #region Variables
        private static Server instance;
        private TcpListener listener;

        private IGame Game;
        private List<Client> Clients;
        public Dictionary<Games, List<Client>> Queue;
        private Dictionary<Client, IGame> ClientGame;
        private List<Thread> Games;
        public Dictionary<Games, IGame> GameTypes;
        private Dictionary<Client, bool> DisconnectedClients;

        private int Port;
        private string Name;
        private int MaxPlayers;

        public Random rng;
        private static bool inputting;

        private GServer RedisGServer;
        private RedisController redisController;
        private AuthRepository authRepo;
        private ServerRepository serverRepo;

        private SQLController sqlController;
        private SqlRepository sqlRepo;
        public AccountRepository accountRepository;

        public Status ServerStatus { get; private set; }

        #endregion

        #region Constructor
        public Server()
        {
            redisController = new RedisController();
            authRepo = new AuthRepository(redisController);
            serverRepo = new ServerRepository(redisController);

            sqlController = new SQLController();
            sqlRepo = new SqlRepository(sqlController);
            accountRepository = new AccountRepository(sqlRepo);

            inputting = false;
            instance = this;

            Games = new List<Thread>();
            Clients = new List<Client>();
            GameTypes = new Dictionary<Games, IGame>();
            ClientGame = new Dictionary<Client, IGame>();
            Queue = new Dictionary<Games, List<Client>>();

            GameTypes.Add(ServerData.src.data.Games.RPS, new RPS(this));
            GameTypes.Add(ServerData.src.data.Games.GUESS, new GUESS(this));

            Queue.Add(ServerData.src.data.Games.RPS, new List<Client>());
            Queue.Add(ServerData.src.data.Games.GUESS, new List<Client>());

            rng = new Random();

            this.Name = Config.serverName;
            this.Port = Config.serverPort;
            MaxPlayers = Config.MaxPlayers;
            ServerStatus = Status.BOOTING;

            Util.Debug($"SERVER STATUS: {ServerStatus}");

            listener = new TcpListener(IPAddress.Any, Port);
        }

        #endregion

        #region Misc
        public void UpdateTitleStatus()
        {
            string TitleStatus = $"GameServer {Name} {ServerStatus} on {Port} with {Clients.Count} {Games.Count} games running.";
            Console.Title = TitleStatus;
        }
        #endregion

        #region Main
        public void Boot()
        {
            List<Task> ConnectionTasks = new List<Task>();
            listener.Start();
            Console.WriteLine($"Game Server started on port {Port}");
            //Starts listener on specified port 
            ServerStatus = Status.RUNNING;
            Util.Debug($"SERVER STATUS: {ServerStatus}");
            //Instantiates new list of Tasks to be run on the server
            RedisGServer = new GServer(1, Name, Port, "default", Clients.Count, Config.MaxPlayers, DateTime.Now, DateTime.Now);

            serverRepo.PostServer(RedisGServer);
            //Server Running
            while (ServerStatus == Status.RUNNING)
            {
                Config.Debug = true;
                //Update Redis Stats
                RedisGServer.players = Clients.Count;
                RedisGServer.lastPing = DateTime.Now;
                RedisGServer.Update(serverRepo);

                //Gets input
                //if (!inputting) { ConsoleInput().GetAwaiter().GetResult(); }

                //Updates title
                UpdateTitleStatus();
                //Checks if there are any join requests on the listener
                if (listener.Pending())
                {
                    ConnectionTasks.Add(OnNewConnection());
                }
                foreach(var g in GameTypes)
                {
                    if (Queue[g.Key].Count >= g.Value.RequiredPlayers)
                    {
                        Util.Debug($"New game starting '{g.Value.GameName}' with the following clients: ");
                        if (Queue[g.Key].Count <= g.Value.MaxPlayers)
                        {
                            foreach (Client c in Queue[g.Key].ToArray())
                            {
                                Util.Debug($" - {c.uuid}"); ;
                                g.Value.AddPlayer(c);
                                Queue[g.Key].Remove(c);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < g.Value.MaxPlayers; i++)
                            {
                                Util.Debug($" - {Queue[g.Key][i].uuid}");
                                g.Value.AddPlayer(Queue[g.Key][i]);
                            }
                        }
                        Thread gameThread = new Thread(new ThreadStart(g.Value.Start));
                        gameThread.Name = $"{g.Value.GameName}";
                        gameThread.Start();
                        Games.Add(gameThread);
                        UpdateTitleStatus();
                    }
                    //for (int i = 0; i < Queue[g.Key].Count; i++)
                    //{
                    //    bool disconnected = false;
                    //    Client client = Queue[g.Key][i];
                    //    disconnected = isDisconnected(client);
                    //    if (disconnected)
                    //    {
                    //        Clients.Remove(client);
                    //        removeClient(client);
                    //        DisconnectedClients[client] = true;
                    //    }
                    //    Thread.Sleep(10);
                    //}
                }
            }
            Task.WaitAll(ConnectionTasks.ToArray(), 1000);
            listener.Stop();
            UpdateTitleStatus();
        }
        #endregion

        #region Commands 
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
            else if (command == "updates" || command == "update")
            {
                Util.Write("Checking for updates..");
                if (UpdateManager.CheckUpdated())
                {
                    UpdateManager.RestartInstance();
                }
                else
                {
                    Util.Write("[UPDATE CHECK] No updates found.");
                }
            }
            else if(command == "redis")
            {
                Util.Write("[DEV MODE] Force updating redis.");
                serverRepo.PostServer(RedisGServer);
                authRepo.UpdateKeys();
            }
            else if (command[0] == 'z')
            {
                string a = command.Split(' ')[1];
                string uuid = authRepo.GetUUID(a);
                Console.WriteLine(uuid);
            }
            else if(command == "debug")
            {
                Util.Write("Enabled debug mode.");
                Config.Debug = true;
            }
        }
        #endregion

        #region Disconnection Handling
        public bool isDisconnected(Client client)
        {
            //Checks if the TcpClient has been disconnected
            //https://stackoverflow.com/questions/722240/instantly-detect-client-disconnection-from-server-socket
            try
            {
                Socket clientSocket = client.tcpClient.Client;
                return clientSocket.Poll(1, SelectMode.SelectRead) && clientSocket.Available == 0;
            }
            catch (SocketException)
            {
                return true;
            }
        }

        public void DisconnectClient(Client client, string msg = "null")
        {
            Console.WriteLine("DISCONNECTED CLIENT");
            Util.Log($"DISCONNECTED CLIENT {client.uuid} with msg {msg}");
            Task DisconnectPacket = instance.SendPacket(client.tcpClient, new Packet("disconnect", msg));
            try
            {
                ClientGame[client].DisconnectClient(client);
            }
            catch (Exception e) { Util.Error(e); Util.Log(e.Message); Console.WriteLine("DEBUG: {0}", e.Message); }
            try
            {
                foreach (var g in Queue)
                {
                    if (g.Value.Contains(client))
                    {
                        g.Value.Remove(client);
                    }
                }
            }
            catch (Exception e)
            {
                Util.Error(e); Util.Log(e.Message); Console.WriteLine("TRIED REMOVING FROM QUEUE: {0}", e.Message);
            }
            DisconnectPacket.GetAwaiter().GetResult();
            DisconnectedClient(client);

        }

        public void DisconnectedClient(Client client)
        {
            Util.Log($"Removed {client.uuid}from client list");
            Clients.Remove(client);
            DestroyClient(client);
        }

        public void DestroyClient(Client client)
        {
            Util.Log($"DESTROYED {client.uuid}from client list");
            client.tcpClient.GetStream().Close();
            client.tcpClient.Close();
        }

        #endregion

        #region Validation & Connection
        public async Task OnNewConnection()
        {
            //Accepts any pending connections 
            TcpClient newClient = await listener.AcceptTcpClientAsync();

            Client client = ValidateConnection(newClient);
            if(client != null)
            {
                Console.WriteLine("SUCCESS");
                Util.Debug($"New connection from {newClient.Client.RemoteEndPoint}");

                //Sends welcome message packet
                string msg = String.Format($"Welcome to the {Name} Game Server");
                Server.SendMessage(client, msg);

                //Adds client to the list of clients
                Clients.Add(client);
            }
        }

        private Client ValidateConnection(TcpClient newTCPClient)
        {
            //Validates that the TCP Connection has been sent from the Client program
            string authToken = RequestAuth(newTCPClient).GetAwaiter().GetResult();
            Client client = null;
            try
            {
                string uuid = authRepo.GetUUID(authToken);
                client = new Client(newTCPClient, uuid);

                SQLController sqlController = new SQLController();
                SqlRepository sqlRepo = new SqlRepository(sqlController);
                AccountRepository accountRepo = new AccountRepository(sqlRepo);
                accountRepo.LoadAccount(client);

                Util.Debug($"Validated client {newTCPClient.Client.RemoteEndPoint}");
                Console.WriteLine($"New client with uuid {uuid} connected from {newTCPClient.Client.RemoteEndPoint}");
                string gameSelection = RequestGame(newTCPClient).GetAwaiter().GetResult();
                Games game;
                Enum.TryParse(gameSelection, out game);
                Queue[game].Add(client);
                return client;
            }
            catch(Exception e)
            {
                Util.Debug($"Failed to validate client {client.tcpClient.Client.RemoteEndPoint} | {e.Message}");
                return client; 
            }
        }
        #endregion

        #region Data
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
        #endregion

        #region Abstraction
        public static void SendMessageAll(List<Client> clients, string msg)
        {
            Util.Debug($"Sent message {msg} to all clients");
            for (int i = 0; i < clients.Count; i++)
            {
                instance.SendPacket(clients[i].tcpClient, new Packet("msg", msg)).GetAwaiter().GetResult();
                Thread.Sleep(15);
            }
        }
        
        public static void SendMessage(Client client, string msg)
        {
            Util.Debug($"Sent message {msg} to all {client}");
            instance.SendPacket(client.tcpClient, new Packet("msg", msg)).GetAwaiter().GetResult();
        }

        public static async Task<string> RequestInput(Client client, string msg)
        {
            string response = "";
            Packet RequestPacket = new Packet("input", msg);
            instance.SendPacket(client.tcpClient, RequestPacket).GetAwaiter().GetResult();
            Util.Debug($"Requested input '{msg}' from [{client.uuid}]");
            await Task.Delay(500);
            Packet Answer = instance.ReceivePacket(client.tcpClient).GetAwaiter().GetResult();
            response += Answer.Content;
            Util.Debug($"Recieved input '{response}' from [{client.uuid}]");
            return response;
        }

        public static async Task<string> RequestAuth(TcpClient client)
        {
            string response = "";
            Packet RequestPacket = new Packet("auth", "auth");
            instance.SendPacket(client, RequestPacket).GetAwaiter().GetResult();
            Util.Debug($"Requested auth '{"auth"}' from [{client.Client.RemoteEndPoint}]");
            await Task.Delay(500);
            Packet Answer = instance.ReceivePacket(client).GetAwaiter().GetResult();
            response += Answer.Content;
            Util.Debug($"Recieved auth '{response}' from [{client.Client.RemoteEndPoint}]");
            return response;
        }

        public static async Task<string> RequestGame(TcpClient client)
        {
            string response = "";
            Packet RequestPacket = new Packet("game", "game");
            instance.SendPacket(client, RequestPacket).GetAwaiter().GetResult();
            Util.Debug($"Requested game selection '{"game"}' from [{client.Client.RemoteEndPoint}]");
            await Task.Delay(500);
            Packet Answer = instance.ReceivePacket(client).GetAwaiter().GetResult();
            response += Answer.Content;
            Util.Debug($"Recieved auth '{response}' from [{client.Client.RemoteEndPoint}]");
            return response;
        }

        public static async Task<Dictionary<Client, string>> RequestInputAll(List<Client> clients, string msg)
        {
            Util.Debug($"Requested input '{msg}' from all players");
            Dictionary<Client, string> result = new Dictionary<Client, string>();
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
        #endregion
    }
}
