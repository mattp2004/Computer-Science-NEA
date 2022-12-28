using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers.server
{
    public class ServerController : ApiController
    {
        [Route("api/server/{token}/{name}")]
        public string GET(string token, string name)
        {
            if (token == Config.AccessToken)
            {
                return DataManager.instance.serverRepo.GetServer(name).ToString();
            }
            return "FAILED";
        }
    }
}
