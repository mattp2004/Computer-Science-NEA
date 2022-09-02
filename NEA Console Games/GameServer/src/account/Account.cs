using GameServer.src.account.stats;
using GameServer.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GameServer.src.account.AccountRepository;

namespace GameServer.src.account
{
    class Account
    {
        private static Dictionary<Client, Account> cache = new Dictionary<Client, Account>();

        private Client Client;
        private string UUID;
        private Stats AccountStats;
        private string Username;
        private Rank UserRank;
        private int Points;

        public Account(ResultSet set, Client _client)
        {
            this.Username = set.username;
            this.UserRank = set.rank;
            this.Points = set.Points;
            cache.Add(_client,this);
        }

        public Account(string name)
        {
            Username = name;
        }
        public Account(string uuid, string name, Rank _rank, int points, Stats accountStats = null)
        {
            UUID = uuid;
            Username = name;
            if (accountStats == null)
            {
                AccountStats = new Stats();
            }
            AccountStats = accountStats;
            UserRank = _rank;
            Points = points;
        }

        public static Dictionary<Client, Account> GetCache()
        {
            return cache;
        }

        public static Account Get(Client _client)
        {
            if(_client == null) { return null; }
            return cache[_client];
        }
        public void remove()
        {
            cache.Remove(Client);
        }

        public string GetName() { return Username; }
        public Stats GetStatsObj() { return AccountStats; }

        public void AddPoints(int num) { Points += num; }
        public void RemovePoints(int num) { Points -= num; }
        public void SetPoints(int num) { Points = num; }


    }
}
