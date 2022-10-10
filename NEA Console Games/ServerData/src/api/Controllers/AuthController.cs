using ServerData.src.data;
using ServerData.src.redis;
using ServerData.src.redis.auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers
{
    public class AuthController : ApiController
    {

        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 
        [Route("api/auth/{uuid}/{auth}/{token}")]
        public string Get(string input, string token)
        {
            if(token == Config.AccessToken)
            {
                string msg;
                msg = DataManager.instance.authRepo.GetUUID(input);
                if (msg == "INVALID")
                {
                    return DataManager.instance.authRepo.GetToken(input);
                }
                return msg;
            }
            return null;
        }

        // POST api/values 
        [Route("api/auth/{uuid}/{auth}/{token}")]
        public string Post(string uuid, string auth, string token)
        {
            if (token == Config.AccessToken)
            {
                DataManager.instance.authRepo.PostAuth(new Auth(uuid, auth));
                return "OK";
            }
            else
            {
                Console.WriteLine("INVALID TOKEN");
            }
            return null;
        }
    }
}
