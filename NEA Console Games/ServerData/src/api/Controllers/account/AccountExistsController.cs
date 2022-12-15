using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers.account
{
    public class AccountExistsController : ApiController
    {
        [Route("api/accountexists/{token}/{username}")]
        public string GET(string token, string username)
        {
            if (token == Config.AccessToken)
            {
                try
                {
                    return "" + DataManager.instance.accountRepository.AccountExists(username);
                }
                catch(Exception e)
                {
                    return ""+false;
                }
            }
            return "FAILED";
        }
    }
}
