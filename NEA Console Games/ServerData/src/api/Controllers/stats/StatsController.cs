using ServerData.src.data;
using ServerData.src.stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServerData.src.api.Controllers.stats
{
    public class StatsController : ApiController
    {
        [Route("api/stats/{token}/{userame}")]
        public string Get(string token, string username)
        {
            if (token == Config.AccessToken)
            {
                return DataManager.instance.statsRepo.GetStatsFromUsername(username).Serialize();
            }
            return "FAILED";
        }

        [Route("api/stats/{token}/{userame}/{game}")]
        public string Get(string token, string username, string type)
        {
            if (token == Config.AccessToken)
            {
                if(type == "web")
                {
                    string toReturn = "";
                    Stats s = DataManager.instance.statsRepo.GetStatsFromUsername(username);
                    foreach (string name in Enum.GetNames(typeof(Games)))
                    {
                        string t = name;
                        Games a;
                        Enum.TryParse(name, out a);
                        toReturn += "<b>" + t + "</b>" + $"\nWins: {s.GameWins[a]}\nLosses: {s.GameLosses[a]}\nPlayed: {s.GamesPlayed[a]}";
                        toReturn += "\n\n";
                    }
                    return toReturn;
                }
            }
            return "FAILED";
        }
    }
}
