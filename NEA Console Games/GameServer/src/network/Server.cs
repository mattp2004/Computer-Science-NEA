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
using ServerData.src.sql.game;

namespace GameServer.src.network
{
    public enum Status
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

        public List<Client> Clients;
        public Dictionary<Games, List<Client>> Queue;
        private Dictionary<Client, IGame> ClientGame;
        public List<Games> GameTypes;
        public Dictionary<Games, List<IGame>> Games;

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
        public SqlGameRepository sqlGameRepository;

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
            sqlGameRepository = new SqlGameRepository(sqlController);

            inputting = false;
            instance = this;

            Games = new Dictionary<Games, List<IGame>>();
            Clients = new List<Client>();
            GameTypes = new List<Games>();
            ClientGame = new Dictionary<Client, IGame>();
            Queue = new Dictionary<Games, List<Client>>();

            GameTypes.Add(ServerData.src.data.Games.RPS);
            GameTypes.Add(ServerData.src.data.Games.GUESS);
            GameTypes.Add(ServerData.src.data.Games.BLACKJACK);

            for(int i = 0; i < GameTypes.Count; i++)
            {
                Games.Add(GameTypes[i], new List<IGame>());
                Queue.Add(GameTypes[i], new List<Client>());
            }

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

        public void MonitorDisconnects()
        {
            while (true)
            {
                foreach (Client c in Clients.ToArray())
                {
                    bool inGame = false;
                    if (ClientGame.ContainsKey(c)){
                        inGame = true;
                    }
                    if (!inGame)
                    {
                        bool disconnected = false;
                        disconnected = isDisconnected(c);
                        if (disconnected)
                        { RemoveDisconnectedClient(c); Console.WriteLine("Disconnecting client " + c.GetAccount().GetUsername()); }
                    }
                }
                Thread.Sleep(10);
            }
        }

        #region Main
        public void Boot()
        {
            //Stores the tasks that will be executed when someone joins.
            List<Task> ConnectionTasks = new List<Task>();

            //Opens listener
            listener.Start();
            ServerStatus = Status.RUNNING;
            Console.WriteLine($"Game Server started on port {Port}");
            Util.Debug($"SERVER STATUS: {ServerStatus}");

            //Redis Server
            RedisGServer = new GServer(1, Name, Port, "default", Clients.Count, Config.MaxPlayers, DateTime.Now, DateTime.Now);
            serverRepo.PostServer(RedisGServer);

            //Async monitoring for disconnects
            List<Thread> Tasks = new List<Thread>();
            Thread dcMonitor = new Thread(MonitorDisconnects);
            dcMonitor.Start();
            Tasks.Add(dcMonitor);

            //Server Running
            while (ServerStatus == Status.RUNNING)
            {
                //Update Redis Stats
                RedisGServer.players = Clients.Count;
                RedisGServer.lastPing = DateTime.Now;
                RedisGServer.Update(serverRepo);

                //Gets input
                //if (!inputting) { ConsoleInput().GetAwaiter().GetResult(); }

                UpdateTitleStatus();

                //Checks if there are any join requests on the listener
                if (listener.Pending())
                {
                    ConnectionTasks.Add(OnNewConnection());
                }

                //Matchmaking
                Matchmaking();
            }
            Task.WaitAll(ConnectionTasks.ToArray(), 1000);
            listener.Stop();
            UpdateTitleStatus();
        }

        private void Matchmaking()
        {
            //Loops through each gametype
            foreach (Games g in GameTypes.ToArray())
            {
                //If someone in queue proceed to check for open games before creating a new one
                if (Queue[g].Count > 0)
                {
                    IGame openGame = null;
                    //Check for open games.
                    for(int i = 0; i < Games[g].Count; i++)
                    {
                        Console.WriteLine("Checking for open Games: " + i + " Status: " + Games[g][i].GetStatus + " with players " + Games[g][i].PlayerCount + "/" + Games[g][i].MaxPlayers);
                        if (Games[g][i].GetStatus == GameStatus.WAITING)
                        {
                            if(Games[g][i].PlayerCount < Games[g][i].MaxPlayers)
                            {
                                Console.WriteLine("Found open game");
                                openGame = Games[g][i];
                            }
                        }
                    }

                    //Fill as many from queue into Open game
                    if (openGame != null)
                    {
                        Console.WriteLine("Filling players into queue as there is a game");
                        foreach (Client c in Queue[g].ToArray())
                        {
                            Console.WriteLine("Player in queuue " + c.GetAccount().GetUsername() + " " + g.ToString());
                            if (openGame.PlayerCount < openGame.MaxPlayers)
                            {
                                Console.WriteLine("ADDING PLAYER TO GAME");
                                openGame.OnPlayerJoin(c);
                                Queue[g].Remove(c);
                                ClientGame.Add(c, openGame);
                            }
                            else break;
                        }
                    }
                    //Create new game
                    else
                    {
                        Console.WriteLine("Creating new game");
                        IGame game = null;
                        if(g == ServerData.src.data.Games.BLACKJACK)
                        {
                            game = new BLACKJACK(this);
                            Console.WriteLine("Created new blackjack game");
                        }
                        else if (g == ServerData.src.data.Games.RPS)
                        {
                            game = new RPS(this);
                            Console.WriteLine("Created new RPS game");
                        }
                        if (game != null)
                        {
                            Thread gameThread = new Thread(new ThreadStart(game.Boot));
                            gameThread.Start();
                            Games[g].Add(game);
                        }
                    }
                }
            }
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
            if (command == "games")
            {
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
            else if(command == "debug")
            {
                Util.Write("Enabled debug mode.");
                Config.Debug = true;
            }
        }
        #endregion

        #region Disconnection Handling
        //TODO: rename and redo
        public bool isDisconnected(Client client)
        {
            //Checks if the TcpClient has been disconnected
            //https://stackoverflow.com/questions/722240/instantly-detect-client-disconnection-from-server-socket
            try
            {
                Socket clientSocket = client.tcpClient.Client;
                return (clientSocket.Poll(1, SelectMode.SelectRead) && clientSocket.Available == 0);
            }
            catch (SocketException)
            {
                Console.WriteLine("DETECTED DISCONNETION [isDisconnected] {0}", client.tcpClient.Client.RemoteEndPoint);
                return true;
            }
        }

        //graceful disconnect server kicks player off when game over etc
        public void Disconnect(Client client, string msg = "null")
        {
            Util.Log($"DISCONNECTED CLIENT {client.uuid} with msg {msg}");

            Task DisconnectPacket = instance.SendPacket(client.tcpClient, new Packet("disconnect", msg));

            try
            {
                ClientGame[client].OnPlayerQuit(client);
                Console.WriteLine("Successfully disconnected client from game");
            }
            catch (Exception e) { Util.Error(e); Util.Log(e.Message); Console.WriteLine("DEBUG: {0}", e.Message); }
            Thread.Sleep(100);
            DisconnectPacket.GetAwaiter().GetResult();
            RemoveDisconnectedClient(client);

        }


        //when player decides to leave before the game is over.
        public void RemoveDisconnectedClient(Client client)
        {
            try
            {
                foreach (var g in Queue)
                {
                    if (g.Value.Contains(client))
                    {
                        g.Value.Remove(client);
                        Console.WriteLine("Removed from queue");
                    }
                }
            }
            catch (Exception e)
            {
                Util.Error(e); Util.Log(e.Message); Console.WriteLine("TRIED REMOVING FROM QUEUE: {0}", e.Message);
            }
            try
            {
                ClientGame[client].OnPlayerQuit(client);
                Console.WriteLine("Successfully disconnected client from game");
            }
            catch (Exception e) { Util.Error(e); Util.Log(e.Message); Console.WriteLine("DEBUG: {0}", e.Message); }
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

                Thread.Sleep(150);
                Util.Debug($"Validated client {newTCPClient.Client.RemoteEndPoint}");
                Console.WriteLine($"New client with uuid {uuid} connected from {newTCPClient.Client.RemoteEndPoint}");
                string gameSelection = RequestGame(newTCPClient).GetAwaiter().GetResult();
                Console.WriteLine(gameSelection);
                Games game;
                try
                {
                    Enum.TryParse(gameSelection, out game);
                    Queue[game].Add(client);
                }
                catch (Exception)
                {
                    //Potential game join code
                    IGame JoinCodeGame = null;
                    foreach(var g in Games)
                    {
                        foreach(IGame ga in g.Value)
                        {
                            if (ga.GetCode == gameSelection)
                            {
                                JoinCodeGame = ga;
                            }
                        }
                    }
                    if(JoinCodeGame != null)
                    {
                        if(JoinCodeGame.PlayerCount < JoinCodeGame.MaxPlayers && JoinCodeGame.GetStatus == GameStatus.WAITING)
                        {
                            JoinCodeGame.OnPlayerJoin(client);
                        }
                    }
                }
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
        public static void UpdateTitle(Client client, string Title)
        {
            instance.SendPacket(client.tcpClient, new Packet("title", Title)).GetAwaiter().GetResult();
        }

        public static void UpdateTitleAll(List<Client> clients, string Title)
        { 
            for(int i = 0; i < clients.Count; i++)
            {
                instance.SendPacket(clients[i].tcpClient, new Packet("title", Title)).GetAwaiter().GetResult();
                Thread.Sleep(10);
            }
        }

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
            if(Answer.Type == "disconnect")
            {
                Server.instance.RemoveDisconnectedClient(client);
            }
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
            Util.Debug($"Requested auth '{"game"}' from [{client.Client.RemoteEndPoint}]");
            await Task.Delay(500);
            Packet Answer = instance.ReceivePacket(client).GetAwaiter().GetResult();
            response += Answer.Content;
            Util.Debug($"Recieved game '{response}' from [{client.Client.RemoteEndPoint}]");
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
