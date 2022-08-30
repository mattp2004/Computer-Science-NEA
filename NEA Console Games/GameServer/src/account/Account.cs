using GameServer.src.account.stats;
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
        private string UUID;
        private Stats AccountStats;
        private string Username;
        private Rank UserRank;
        private int Points;

        public Account(ResultSet set)
        {
            this.Username = set.username;
            this.UserRank = set.rank;
            this.Points = set.Points;
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

        public string GetName() { return Username; }
        public Stats GetStatsObj() { return AccountStats; }

        public void AddPoints(int num) { Points += num; }
        public void RemovePoints(int num) { Points -= num; }
        public void SetPoints(int num) { Points = num; }


    }
}
