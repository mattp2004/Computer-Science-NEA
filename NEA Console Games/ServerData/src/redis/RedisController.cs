using ServerData.src.api.Controllers.util;
using ServerData.src.misc;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.redis
{
    public enum RedisStatus
    {
        CONNECTING,
        CONNECTED,
        DISCONNECTED,
    }
    public class RedisController
    {
        public IDatabase database { get; }
        public RedisStatus status { get; set; }
        public RedisController()
        {
            status = RedisStatus.DISCONNECTED;
            try
            {
                var conn = ConnectionMultiplexer.Connect($"localhost:{RedisConfig.PORT},password={RedisConfig.AUTH}");
                database = conn.GetDatabase();
                if (conn.IsConnected)
                {
                    TextUtil.WriteLine($"Connected to Redis Database.", ConsoleColor.Green);
                    status = RedisStatus.CONNECTED;
                }
            }
            catch(Exception e) { Util.Error(e); }
        }

    }
}
