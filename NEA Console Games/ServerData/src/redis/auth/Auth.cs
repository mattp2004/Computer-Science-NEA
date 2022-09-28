using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerData.src.redis.auth
{
    public class Auth
    {
        public string uuid;
        public string token;

        public Auth(string uuid, string token)
        {
            this.uuid = uuid;
            this.token = token;
        }
    }
}
