using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers.util
{
    public class TokenController : ApiController
    {
        [Route("api/token/{token}/{count}")]
        public IEnumerable<string> Get(string token, int count = 1)
        {
            if (token == Config.AccessToken)
            {
                string[] uuids = new string[count];
                for (int i = 0; i < count; i++)
                {
                    uuids[i] = DataUtil.GenerateToken();
                }
                return uuids;
            }
            return new string[] { "null" };
        }
    }
}
