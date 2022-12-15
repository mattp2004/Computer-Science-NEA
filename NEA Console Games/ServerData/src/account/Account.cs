using Newtonsoft.Json;
using ServerData.src.network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServerData.src.account.AccountRepository;

namespace ServerData.src.account
{
    public class Account
    {
        private static Dictionary<Client, Account> cache = new Dictionary<Client, Account>();

        private Client Client { get; set; }
        private int id { get;}
        private string UUID { get; }
        private string Username { get; }
        private string Password { get; }
        private Rank UserRank { get; }
        private int Tokens { get;}

        public Account(AccountSet set, Client _client)
        {
            this.Client = _client;
            this.id = set.id;
            this.UUID = set.uuid;
            this.Username = set.username;
            this.Password = set.password;
            this.UserRank = set.rank;
            this.Tokens = set.tokens;
            if (cache.ContainsKey(_client))
            {
                cache[_client] = this;
            }
            else
            {
                cache.Add(_client, this);
            }
        }

        public static Dictionary<Client, Account> GetCache()
        {
            return cache;
        }

        public static Account Get(Client _client)
        {
            if (_client == null) { return null; }
            return cache[_client];
        }

        public void remove()
        {
            cache.Remove(Client);
        }

        public int GetID() { return id; }
        public string GetUuid() { return UUID; }
        public string GetUsername() { return Username; }
        public string GetPassword() { return Password; }
        public Rank GetRank() { return UserRank; }
        public int GetTokens() { return Tokens; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override string ToString()
        {
            return "Account{" +
                    "id=" + id +
                    ", uuid=" + UUID +
                    ", username='" + Username + '\'' +
                    ", password='" + Password + '\'' +
                    ", tokens=" + Tokens +
                    ", rank=" + UserRank +
                    '}';
        }

        //public Stats GetStatsObj() { return AccountStats; }

        //public void AddPoints(int num) { Tokens += num; }
        //public void RemovePoints(int num) { Tokens -= num; }
        //public void SetPoints(int num) { Tokens = num; }


    }
}
