using ServerData.src.account;
using ServerData.src.data;
using ServerData.src.redis.auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers.account
{
    public class LoginController : ApiController
    {
        [Route("api/login/{token}/{username}/{password}")]
        public string GET(string token, string username, string password)
        {
            if (token == Config.AccessToken)
            {
                AccountSet a = new AccountSet();
                try
                {
                    a = DataManager.instance.accountRepository.GetAccountFromUsername(username);
                }
                catch(Exception e) { Console.WriteLine(e.Message); }
                Console.WriteLine(a.id + " " + a.username);
                string uuid = a.uuid;
                string dbPass = a.password;
                if(password == dbPass)
                {
                    string accessToken = DataUtil.GenerateToken();
                    Auth tempAuth = new Auth(uuid, accessToken);

                    DataManager.instance.authRepo.PostAuth(tempAuth);
                    return accessToken;
                }
                else
                {
                    Console.WriteLine("INVALID");
                    return "FAILED";
                }
            }
            return "FAILED";
        }
    }
}
