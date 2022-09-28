using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.redis.auth
{
    public class AuthRepository
    {
        public RedisController redisController;
        private HashEntry[] Hashes;
        private string KeyName = "Auth";
        public AuthRepository(RedisController redisController)
        {
            this.redisController = redisController;
            Hashes = new HashEntry[100];
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
            redisController.database.HashSet(KeyName, Hashes);
        }

        public string GetUUID(string token)
        {
            return redisController.database.HashGet(KeyName, token);
        }

        public string GetToken(string UUID)
        {
            return redisController.database.HashGet(KeyName, UUID);
        }
    }
}
