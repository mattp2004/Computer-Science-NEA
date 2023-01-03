using GameServer.src.misc;
using GameServer.src.network;
using ServerData.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.src.game.impl
{

    enum Choices
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3,
    }

    class RPS : IGame
    {
        #region Properties
        public string GameName
        {
            get { return "Rock, Paper, Scissors"; } //Get from SQL
        }

        public int RequiredPlayers
        {
            get { return 2; } //get from sql
        }

        public int MaxPlayers
        {
            get { return 2; } // get from sql;
        }

        public int PlayerCount
        {
            get { return Players.Count; }
        }

        public GameStatus GetStatus
        {
            get { return Status; }
        }

        public string GetCode
        {
            get { return Code; }
        }
        #endregion

        public Server Instance;
        public List<Client> Players;
        public GameStatus Status;
        public string Code;
        public int CountdownTime = 30;
        public int CurrentCountdown;

        public int Rounds = 3;
        public int CurrentRound = 1;

        public long StartTime;
        public long EndTime;

        public Dictionary<string, string> PlayerPlace;


        public RPS(Server instance)
        {
            Instance = instance;
            Players = new List<Client>();
            Code = ServerData.src.data.DataUtil.GenerateToken();
            CurrentCountdown = CountdownTime;
            PlayerPlace = new Dictionary<string, string>();
        }

        public void Boot()
        {
            Status = GameStatus.WAITING;
            while (Status == GameStatus.WAITING)
            {
                if (Players.Count >= RequiredPlayers)
                {
                    StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    Status = GameStatus.STARTING;
                    Start();
                }
            }
        }
        public void Start()
        {
            Status = GameStatus.PLAYING;
            while (Status == GameStatus.PLAYING)
            {
                Console.WriteLine("Triggered Game Start");
                CheckEnd();
                Dictionary<Client, Choices> Responses = GetRPSInputAll(Players).GetAwaiter().GetResult();
                bool end = false;
                foreach(var a in Responses)
                {
                    if (!Players.Contains(a.Key))
                    {
                        end = true;
                        CheckEnd();
                        Responses.Remove(a.Key);
                    }
                }
                if (!end) 
                {
                    if (Responses.Count > 1)
                    {
                        if (Responses[Players[0]] == Choices.Rock)
                        {
                            if (Responses[Players[1]] == Choices.Scissors)
                            {
                                Server.SendMessageAll(Players, $"{Players[0].GetAccount().GetUsername()} WON");
                                PlayerPlace.Add(Players[0].GetAccount().GetID().ToString(), "1");
                            }
                            else if (Responses[Players[1]] == Choices.Paper)
                            {
                                Server.SendMessageAll(Players, $"{Players[1].GetAccount().GetUsername()} WON");
                                PlayerPlace.Add(Players[1].GetAccount().GetID().ToString(), "1");
                            }
                            else
                            {
                                Server.SendMessageAll(Players, $"IT'S A DRAW");
                            }
                        }
                        else if (Responses[Players[0]] == Choices.Scissors)
                        {
                            if (Responses[Players[1]] == Choices.Paper)
                            {
                                Server.SendMessageAll(Players, $"{Players[0].GetAccount().GetUsername()} WON");
                                PlayerPlace.Add(Players[0].GetAccount().GetID().ToString(), "1");
                            }
                            else if (Responses[Players[1]] == Choices.Rock)
                            {
                                Server.SendMessageAll(Players, $"{Players[1].GetAccount().GetUsername()} WON");
                                PlayerPlace.Add(Players[1].GetAccount().GetID().ToString(), "1");
                            }
                            else
                            {
                                Server.SendMessageAll(Players, $"IT'S A DRAW");
                            }
                        }
                        else if (Responses[Players[0]] == Choices.Paper)
                        {
                            if (Responses[Players[1]] == Choices.Rock)
                            {
                                Server.SendMessageAll(Players, $"{Players[0].GetAccount().GetUsername()} WON");
                                PlayerPlace.Add(Players[0].GetAccount().GetID().ToString(), "1");
                            }
                            else if (Responses[Players[1]] == Choices.Scissors)
                            {
                                Server.SendMessageAll(Players, $"{Players[1].GetAccount().GetUsername()} WON");
                                PlayerPlace.Add(Players[1].GetAccount().GetID().ToString(), "1");
                            }
                            else
                            {
                                Server.SendMessageAll(Players, $"IT'S A DRAW");
                            }
                        }
                        if(PlayerPlace.Count < 1)
                        {
                            foreach(Client c in Players)
                            {
                                PlayerPlace.Add(c.GetAccount().GetID().ToString(), "1");
                            }
                        }
                    }
                }
                End();
            }
            End();
        }

        public async Task<Dictionary<Client, Choices>> GetRPSInputAll(List<Client> clients)
        {
            Dictionary<Client, Choices> result = new Dictionary<Client, Choices>();
            List<Task> InputTasks = new List<Task>();
            for (int i = 0; i < clients.Count; i++)
            {
                Console.WriteLine(i);
                InputTasks.Add(GetRPSInput(clients[i]));
            }
            await Task.WhenAll(InputTasks);
            for (int i = 0; i < clients.Count; i++)
            {
                //https://briancaos.wordpress.com/2021/02/15/c-get-results-from-task-whenall/
                var choiceResponse = ((Task<Choices>)InputTasks[i]).Result;
                result.Add(clients[i], choiceResponse);
            }
            return result;
        }

        public async Task<Choices> GetRPSInput(Client c)
        {
            bool valid = false;
            Choices choice = Choices.Scissors;
            while (!valid)
            {
                if (Players.Contains(c) && c.tcpClient.Connected)
                {
                    string input = Server.RequestInput(c, "Please pick either Rock, Paper or Scissors").GetAwaiter().GetResult();
                    try
                    {
                        Enum.TryParse(input, out choice);
                        valid = true;
                    }
                    catch (Exception e)
                    {
                        valid = false;
                        Server.SendMessage(c, "Invalid Input.");
                    }
                }
                else { CheckEnd(); }
            }
            return choice;
        }

        public void End()
        {
            Status = GameStatus.ENDING;
            EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Instance.sqlGameRepository.PostGame(ServerData.src.data.Games.RPS, PlayerPlace, StartTime, EndTime);
            foreach(Client a in Players)
            {
                Instance.accountRepository.GiveTokens(a.GetAccount(), 250);
                Server.SendMessageAll(Players, "You have been awarded 250 tokens for playing!");
            }
            Thread.Sleep(5000);
            foreach (Client a in Players)
            {
                Instance.Clients.Remove(a);
                Instance.DisconnectClient(a, "Game ending...");
                Thread.Sleep(10);
            }
            Players.Clear();
            Thread.Sleep(1000);
        }

        public bool OnPlayerJoin(Client player)
        {
            if (!Players.Contains(player))
            {
                if (PlayerCount < MaxPlayers)
                {
                    Players.Add(player);
                    return true;
                }
            }
            return false;
        }

        public void OnPlayerQuit(Client player)
        {
            Players.Remove(player);
            CheckEnd();
        }

        public bool CheckEnd()
        {
            if (Players.Count < RequiredPlayers)
            {
                if (Status == GameStatus.PLAYING)
                {
                    End();
                    return true;
                }
                if (Status == GameStatus.STARTING || Status == GameStatus.WAITING)
                {
                    CurrentCountdown = CountdownTime;
                    return false;
                }
            }
            return false;
        }
    }
}
