using ServerData.src.account;
using ServerData.src.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers.account
{
    public class CreateAccountController : ApiController
    {
        [Route("api/createaccount/{token}/{username}/password")]
        public void POST(string token, string username, string password)
        {
            if(token == Config.AccessToken)
            {
                DataManager.instance.accountRepository.CreateAccount(username, password);
            }
        }
    }
}
