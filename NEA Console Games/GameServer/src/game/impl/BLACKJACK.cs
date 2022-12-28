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
        #endregion

        public Server Instance;
        public List<Client> Players;
        public GameStatus Status;

        public long StartTime;
        public long EndTime;

        public BLACKJACK(Server instance)
        {
            Instance = instance;
            Players = new List<Client>();
        }

        public void Boot()
        {
            Status = GameStatus.WAITING;
            while(Status == GameStatus.WAITING)
            {
                if (Players.Count > 1)
                {
                    StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    Console.WriteLine("Game start");
                    Status = GameStatus.STARTING;
                    Thread.Sleep(5000);
                    Status = GameStatus.PLAYING;
                    End();
                }
            }
        }

        public void End()
        {
            Dictionary<string, string> PlayerPlace = new Dictionary<string, string>();

            Status = GameStatus.STARTING;
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
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}
