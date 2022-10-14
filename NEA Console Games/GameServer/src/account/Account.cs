//using GameServer.src.account.stats;
//using GameServer.src.network;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static GameServer.src.account.AccountRepository;

//namespace GameServer.src.account
//{
//    class Account
//    {
//        private static Dictionary<Client, Account> cache = new Dictionary<Client, Account>();

//        private Client Client;
//        private int id;
//        private string UUID;
//        private string Username;
//        private string Password;
//        private Rank UserRank;
//        private int Tokens;

//        public Account(ResultSet set, Client _client)
//        {
//            this.Client = _client;
//            this.id = set.id;
//            this.UUID = set.uuid;
//            this.Username = set.username;
//            this.Password = set.password;
//            this.UserRank = set.rank;
//            this.Tokens = set.tokens;
//            cache.Add(_client,this);
//        }
//        //public Account(string uuid, string name, Rank _rank, int points, Stats accountStats = null)
//        //{
//        //    UUID = uuid;
//        //    Username = name;
//        //    if (accountStats == null)
//        //    {
//        //        AccountStats = new Stats();
//        //    }
//        //    AccountStats = accountStats;
//        //    UserRank = _rank;
//        //    Points = points;
//        //}

//        public static Dictionary<Client, Account> GetCache()
//        {
//            return cache;
//        }

//        public static Account Get(Client _client)
//        {
//            if(_client == null) { return null; }
//            return cache[_client];
//        }

//        public void remove()
//        {
//            cache.Remove(Client);
//        }

//        public string GetName() { return Username; }
//        //public Stats GetStatsObj() { return AccountStats; }

//        //public void AddPoints(int num) { Tokens += num; }
//        //public void RemovePoints(int num) { Tokens -= num; }
//        //public void SetPoints(int num) { Tokens = num; }


//    }
//}
