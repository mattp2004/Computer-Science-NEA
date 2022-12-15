using MySql.Data.MySqlClient.Memcached;
using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers.account
{
    public class AccountViewController : ApiController
    {
        [Route("api/accountview/{token}/{username}")]
        public string GET(string token, string username)
        {
            if (token == Config.AccessToken)
            {
                return DataManager.instance.accountRepository.GetAccountFromUsername(username).ToJson();
            }
            return "FAILED";
        }
    }
}
