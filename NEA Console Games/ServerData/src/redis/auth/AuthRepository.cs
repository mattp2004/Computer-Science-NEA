using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.redis.auth
{
    public class AuthRepository
    {
        public RedisController redisController;
        private static Dictionary<string, long> hashExpire = new Dictionary<string, long>();
        private string KeyName = "Auth";
        private long AuthExpiry = 900000; //miliseconds
        public AuthRepository(RedisController redisController)
        {
            this.redisController = redisController;
            CreateHash();
        }

        private void CreateHash()
        {
            HashEntry[] t = new HashEntry[1];
            t[0] = new HashEntry("UUID", "TOKEN");
            if (!redisController.database.KeyExists(KeyName))
            {
                redisController.database.HashSet(KeyName, t);
            }
        }

        public void PostAuth(Auth auth)
        {
            HashEntry[] Hashes = new HashEntry[1];
            Hashes[0] = new HashEntry(auth.uuid, auth.token);
            AuthRepository.hashExpire.Add(Hashes[0].Value, DateTimeOffset.Now.ToUnixTimeMilliseconds());
            Console.WriteLine($"Added {Hashes[0].Value} | {DateTime.Now.Millisecond}  | {hashExpire[Hashes[0].Value]}");
            Console.WriteLine(hashExpire[Hashes[0].Value]);
            redisController.database.HashSet(KeyName, Hashes);
        }

        public string GetUUID(string token)
        {
            HashEntry[] Hashes = redisController.database.HashGetAll(KeyName);
            for(int i = 0; i < Hashes.Length; i++)
            {
                if(Hashes[i].Value == token)
                {
                    return Hashes[i].Name;
                }
            }
            return "INVALID";
        }

        public void UpdateKeys()
        {
            HashEntry[] hashes = redisController.database.HashGetAll(KeyName);
            List<HashEntry> newHashes = new List<HashEntry>();
            for (int i = 0; i < hashes.Length; i++)
            {
                HashEntry t = hashes[i];
                if(t.Name == "UUID") { continue; }
                try
                {
                    if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - AuthRepository.hashExpire[t.Value] < AuthExpiry)
                    {
                        newHashes.Add(t);
                    }
                }
                catch(Exception e) { }
            }
            HashEntry[] tempHash;
            if (newHashes.Count > 0)
            {
                tempHash = new HashEntry[newHashes.Count+1];
                tempHash[0] = new HashEntry("UUID", "TOKEN");
                try
                {
                    for (int i = 0; i < newHashes.Count; i++)
                    {
                        tempHash[i+1] = newHashes[i];
                    }
                }
                catch(Exception e)
                {
                }
                redisController.database.KeyDelete(KeyName);
                redisController.database.HashSet(KeyName, tempHash);
            }
            else
            {
                HashEntry[] t = new HashEntry[1];
                t[0] = new HashEntry("UUID", "TOKEN");
                redisController.database.KeyDelete(KeyName);
                redisController.database.HashSet(KeyName, t);
            }
        }

        public string GetToken(string UUID)
        {
            return redisController.database.HashGet(KeyName, UUID);
        }
    }
}
