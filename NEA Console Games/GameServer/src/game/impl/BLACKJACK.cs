using GameClient.src.data;
using GameServer.src.data;
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
    class BLACKJACK : IGame
    {
        #region Properties
        public string GameName
        {
            get { return "Blackjack"; } //Get from SQL
        }

        public int RequiredPlayers
        {
            get { return 2; } //get from sql
        }

        public int MaxPlayers
        {
            get { return 6; } // get from sql;
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

        public long StartTime;
        public long EndTime;

        public BLACKJACK(Server instance)
        {
            Instance = instance;
            Players = new List<Client>();
            Code = ServerData.src.data.DataUtil.GenerateToken();
            CurrentCountdown = CountdownTime;
        }

        public void Boot()
        {
            Status = GameStatus.WAITING;
            while(Status == GameStatus.WAITING)
            {
                if (Players.Count > RequiredPlayers)
                {
                    CurrentCountdown--;
                    Thread.Sleep(1000);
                }
                else { CurrentCountdown = CountdownTime; continue; }
                if(CurrentCountdown <= 0)
                {
                    StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    Status = GameStatus.STARTING;
                    Start();
                }
            }
        }
        public void Start()
        {
            Status = GameStatus.STARTING;
            while (Status == GameStatus.PLAYING)
            {

            }
            End();
        }

        public void End()
        {
            Dictionary<string, string> PlayerPlace = new Dictionary<string, string>();

            Status = GameStatus.ENDING;
            foreach (Client c in Players)
            {
                PlayerPlace.Add(c.GetAccount().GetID().ToString(), "2");
                //Instance.DisconnectClient(c, "Thanks for playing");
                //Thread.Sleep(10);
            }
            Players.Clear();
            EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Instance.sqlGameRepository.PostGame(ServerData.src.data.Games.BLACKJACK, PlayerPlace, StartTime, EndTime);
        }

        public bool OnPlayerJoin(Client player)
        {
            if (!Players.Contains(player))
            {
                Players.Add(player);
                Server.SendMessage(player, "BINGO");
                return true;
            }
            return false;
        }

        public void OnPlayerQuit(Client player)
        {
            Players.Remove(player);
            if(Players.Count < RequiredPlayers)
            {
                if(Status == GameStatus.PLAYING)
                {
                    End();
                }
                if (Status == GameStatus.STARTING || Status == GameStatus.WAITING)
                {
                    CurrentCountdown = CountdownTime; 
                }
            }
        }
    }
}
