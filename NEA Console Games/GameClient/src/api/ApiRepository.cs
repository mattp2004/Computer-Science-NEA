using ServerData.src.account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.src.api
{
    public class ApiRepository
    {
        public ApiController apiController;

        public ApiRepository(ApiController apiController)
        {
            this.apiController = apiController;
        }

        public string GenerateToken()
        {
            string uuid = apiController.GET($"http://{ApiConfig.baseAddress}/api/token?token={ApiConfig.authKey}");
            return uuid.Split('"')[1];
        }

        public string GenerateUUID()
        {
            string uuid = apiController.GET($"http://{ApiConfig.baseAddress}/api/uuid?token={ApiConfig.authKey}");
            return uuid.Split('"')[1];
        }

        public void SetAuth(string uuid, string auth)
        {
            apiController.POST($"http://{ApiConfig.baseAddress}/api/auth?uuid={uuid}&auth={auth}&token={ApiConfig.authKey}");
        }

        public void CreateAccount(string username, string password)
        {
            apiController.POST($"http://{ApiConfig.baseAddress}/api/createaccount?token={ApiConfig.authKey}&username={username}&password={password}");
        }
        public string Login(string username, string password)
        {
            string t = apiController.GET($"http://{ApiConfig.baseAddress}/api/login?token={ApiConfig.authKey}&username={username}&password={password}");
            return t;
        }
    } 
}
